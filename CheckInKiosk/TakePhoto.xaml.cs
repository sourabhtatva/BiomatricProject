using AForge.Video;
using AForge.Video.DirectShow;
using CheckInKiosk.Utils.Constants;
using CheckInKiosk.Utils.Models;
using CheckInKiosk.Utils.Resources.ApplicationData;
using CheckInKiosk.Utils.Services;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CheckInKiosk
{
    public partial class TakePhoto : UserControl
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap capturedImage;
        private CancellationTokenSource cts;
        private DispatcherTimer captureTimer;
        private bool faceDetected = false;
        private CascadeClassifier faceCascade;
        private HttpClientService _httpClientService;
        private LogConfig _logConfig;
        private string logConfigPath;


        public TakePhoto()
        {
            InitializeComponent();
            LoadLogConfig();

            try
            {
                faceCascade = new CascadeClassifier(UIConstants.Haarcascade_Frontalface_Path);
            }
            catch (Exception ex)
            {
                LogError(string.Format(_logConfig.LogMessages.FaceCascadeErrorMessage, ex.Message));
                return; // Exit if the face cascade is not properly loaded
            }

            // Initialize the timer
            captureTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            captureTimer.Tick += CaptureTimer_Tick;

            // Initialize CancellationTokenSource
            cts = new CancellationTokenSource();
        }

        public TakePhoto(HttpClientService httpClientService) : this()
        {
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
        }

        private void LoadLogConfig()
        {
            logConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logConfig.json");
            _logConfig = LogConfig.Load(logConfigPath);
        }

        private void LogError(string message)
        {
            Console.WriteLine(message);
        }

        // Method to set the HttpClientService after instantiation
        public void SetHttpClientService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
        }

        private void CaptureTimer_Tick(object sender, EventArgs e)
        {
            if (faceDetected && capturedImage != null)
            {
                captureTimer.Stop();
                CaptureAndVerify(); // Trigger the capture click method
            }
        }

        public void StartCamera()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count > 0)
                {
                    videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                    videoSource.NewFrame += OnNewFrame;
                    videoSource.Start();
                }
                else
                {
                    LogError(_logConfig.LogMessages.CameraNotFoundMessage);
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format(_logConfig.LogMessages.CameraStartErrorMessage, ex.Message));
            }
        }

        public void StopCamera()
        {
            try
            {
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource.NewFrame -= OnNewFrame;
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format(_logConfig.LogMessages.CameraStopErrorMessage, ex.Message));
            }
            finally
            {
                if (videoSource != null)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }

                captureTimer?.Stop();
                captureTimer = null;
            }
        }

        private void OnNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (videoSource == null || !videoSource.IsRunning || cts?.Token.IsCancellationRequested == true)
                {
                    return;
                }

                capturedImage = (Bitmap)eventArgs.Frame.Clone();

                using (var imageFrame = capturedImage.ToImage<Bgr, byte>())
                {
                    var grayFrame = imageFrame.Convert<Gray, byte>();
                    var faces = faceCascade.DetectMultiScale(grayFrame, 1.1, 10, System.Drawing.Size.Empty);

                    foreach (var face in faces)
                    {
                        imageFrame.Draw(face, new Bgr(System.Drawing.Color.Red), 2);
                    }

                    WebcamFeed.Dispatcher.Invoke(() =>
                    {
                        WebcamFeed.Source = BitmapToImageSource(imageFrame.ToBitmap());
                    });

                    if (faces.Length > 0 && !faceDetected)
                    {
                        faceDetected = true;
                        captureTimer.Start(); // Start the timer when a face is detected
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format(_logConfig.LogMessages.NewFrameProcessingErrorMessage, ex.Message));
            }
        }

        private BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Bmp);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format(_logConfig.LogMessages.BitmapConversionErrorMessage, ex.Message));
                return null;
            }
        }

        private void CaptureAndVerify()
        {
            try
            {
                // Stop the camera once the image is captured
                StopCamera();

                WebcamFeed.Visibility = Visibility.Collapsed;
                ShowLoadingOverlay();
                VerificationMessage.Text = _logConfig.LogMessages.IdentityVerificationInProgressMessage;
                VerificationMessage.Visibility = Visibility.Visible;
                CapturePhotoTitle.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Collapsed;

                byte[] scannedImageData = Convert.FromBase64String(ApplicationData.DocumentScannedImage);
                byte[] encryptedScannedImageData = Encryptor.EncryptByteArray(scannedImageData);
                string documentScannedImageBase64String = Convert.ToBase64String(encryptedScannedImageData);

                if (capturedImage == null)
                {
                    LogError(string.Format(_logConfig.LogMessages.CameraStartErrorMessage, "Camera failed to capture an image."));
                    return;
                }

                string clickedImageDataBase64String = BitmapToBase64String(capturedImage);

                byte[] clickedImageData = Convert.FromBase64String(clickedImageDataBase64String);
                byte[] encryptedImageData = Encryptor.EncryptByteArray(clickedImageData);
                string encryptedImageBase64String = Convert.ToBase64String(encryptedImageData);

                cts?.Cancel(); // Cancel any previous task
                cts = new CancellationTokenSource();
                var token = cts.Token;

                Task.Run(async () =>
                {
                    try
                    {
                        var verifyImageResponse = await VerifyImageAsync(encryptedImageBase64String, documentScannedImageBase64String);

                        Dispatcher.Invoke(() =>
                        {
                            HideLoadingOverlay();
                            VerificationMessage.Visibility = Visibility.Collapsed;

                            if (verifyImageResponse.IsIdentical)
                            {
                                CheckInComplete.Visibility = Visibility.Visible;
                            }
                            else if (verifyImageResponse.ApiFailedStatus)
                            {
                                ApiFailedCase.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ManualCheckInPanel.Visibility = Visibility.Visible;
                            }
                        });
                    }
                    catch (TaskCanceledException)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            LogError(_logConfig.LogMessages.VerificationCancelMessage);
                            HideLoadingOverlay();
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            LogError(string.Format(_logConfig.LogMessages.VerificationErrorMessage, ex.Message));
                            HideLoadingOverlay();
                        });
                    }
                }, token);
            }
            catch (Exception ex)
            {
                LogError(string.Format(_logConfig.LogMessages.CaptureVerifyErrorMessage, ex.Message));
                HideLoadingOverlay();
            }
        }

        private async Task<FaceVerifyResponseUI> VerifyImageAsync(string clickImage, string scanImage)
        {
            try
            {
                FaceVerifyResponseUI response = new FaceVerifyResponseUI();

                var request = new MatchFacesRequestUI()
                {
                    ClickedImage = clickImage,
                    ScannedImage = scanImage
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    UIConstants.CONTENT_TYPE
                );

                var responseData = await _httpClientService.PostAsync(APIEndpoint.FACE_MATCHING_API, jsonContent);

                var jsonDocument = JsonDocument.Parse(responseData);
                var data = jsonDocument.RootElement.GetProperty("data");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<FaceVerifyResponseUI>(data, options);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(_logConfig.LogMessages.VerificationApiErrorMessage, ex.Message));
            }
        }

        private void OnOkayClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.RestartApplication();
            }
        }

        private string BitmapToBase64String(Bitmap bitmapcapturedImage)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmapcapturedImage.Save(ms, ImageFormat.Png);
                    byte[] imageBytes = ms.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format(_logConfig.LogMessages.BitmapConversionErrorMessage, ex.Message));
                return null;
            }
        }

        private void ShowLoadingOverlay()
        {
            LoadingOverlay.Visibility = Visibility.Visible;
        }

        private void HideLoadingOverlay()
        {
            LoadingOverlay.Visibility = Visibility.Collapsed;
        }
    }
}
