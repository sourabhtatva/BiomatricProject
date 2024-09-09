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



        public TakePhoto()
        {
            InitializeComponent();
            faceCascade = new CascadeClassifier(UIConstants.Haarcascade_Frontalface_Path);

            // Initialize the timer
            captureTimer = new DispatcherTimer();
            captureTimer.Interval = TimeSpan.FromSeconds(3); // Set delay for 3 seconds
            captureTimer.Tick += CaptureTimer_Tick;

            // Initialize CancellationTokenSource
            cts = new CancellationTokenSource();
        }

        public TakePhoto(HttpClientService httpClientService) : this()
        {
            _httpClientService = httpClientService;
        }

        // Method to set the HttpClientService after instantiation
        public void SetHttpClientService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
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
                    MessageBox.Show("No camera found!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting camera: {ex.Message}");
            }
        }

        public void StopCamera()
        {
            if (videoSource != null)
            {
                try
                {
                    if (videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();
                        videoSource.WaitForStop();
                        videoSource.NewFrame -= OnNewFrame;
                    }
                }
                catch
                {
                    // Handle any exception silently
                }
                finally
                {
                    videoSource = null;
                }
            }

            // Ensure the timer is stopped
            captureTimer.Stop();
        }


        private void OnNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Check if the operation is canceled or the video source is not running
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
                        imageFrame.Draw(face, new Bgr(Color.Red), 2);
                    }

                    WebcamFeed.Dispatcher.Invoke(() =>
                    {
                        WebcamFeed.Source = BitmapToImageSource(imageFrame.ToBitmap());
                    });

                    if (faces.Length > 0)
                    {
                        if (!faceDetected)
                        {
                            faceDetected = true;
                            captureTimer.Start(); // Start the timer when a face is detected
                        }
                    }
                }
            }
            catch
            {
                // Handle any exception silently to avoid interfering with application shutdown
            }
        }


        private static BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Seek(0, SeekOrigin.Begin);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void CaptureAndVerify()
        {
            // Hide the WebcamFeed and other UI elements
            WebcamFeed.Visibility = Visibility.Collapsed;

            // Show Verification Message and Loading Indicator
            VerificationMessage.Text = "We are verifying your identity. Please wait...";
            VerificationMessage.Visibility = Visibility.Visible;
            LoadingOverlay.Visibility = Visibility.Visible;
            CapturePhotoTitle.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Collapsed;

            string documentScannedImageBase64String = ApplicationData.DocumentScannedImage;

            string clickedImageDataBase64String = BitmapToBase64String(capturedImage);

            byte[] clickedImageData = Convert.FromBase64String(clickedImageDataBase64String);
            byte[] encryptedImageData = Encryptor.EncryptByteArray(clickedImageData);
            string encryptedImageBase64String = Convert.ToBase64String(encryptedImageData);

            // Cancel any previous ongoing task
            cts?.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    var verificationSuccess = await VerifyImageAsync(encryptedImageBase64String, documentScannedImageBase64String);

                    Dispatcher.Invoke(() =>
                    {
                        if (verificationSuccess)
                        {
                            VerificationMessage.Visibility = Visibility.Collapsed;
                            LoadingOverlay.Visibility = Visibility.Collapsed;
                            CheckInComplete.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            VerificationMessage.Visibility = Visibility.Collapsed;
                            LoadingOverlay.Visibility = Visibility.Collapsed;
                            CheckInComplete.Visibility = Visibility.Collapsed;
                            ManualCheckInPanel.Visibility = Visibility.Visible;
                        }
                    });
                }
                catch (TaskCanceledException)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("The verification process was canceled.");
                        LoadingOverlay.Visibility = Visibility.Collapsed;
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error during verification: {ex.Message}");
                        LoadingOverlay.Visibility = Visibility.Collapsed;
                    });
                }
            }, token); // Pass the cancellation token here
        }

        private async Task<bool> VerifyImageAsync(string clickImage, string scanImage)
        {
            try
            {
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

                // Use the custom HttpClientService to send the POST request
                var responseData = await _httpClientService.PostAsync(APIEndpoint.FACE_MATCHING_API, jsonContent);

                // Check if responseData is null, indicating a failed request
                if (responseData == null)
                {
                    return false;
                }

                // Parse the response data to extract the 'data' field
                var jsonDocument = JsonDocument.Parse(responseData);
                var data = jsonDocument.RootElement.GetProperty("data");
                bool isVerified = data.GetProperty("isIdentical").GetBoolean();

                return isVerified;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Verification API error: {ex.Message}");
            }
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return memoryStream.ToArray();
            }
        }

        private void OnOkayClick(object sender, RoutedEventArgs e)
        {
            // Notify the MainWindow to restart the application
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.RestartApplication();
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StopCamera();
            cts?.Cancel(); // Cancel any ongoing tasks
            cts?.Dispose(); // Dispose of the cancellation token source
        }

        private string BitmapToBase64String(Bitmap bitmapcapturedImage)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmapcapturedImage.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

    }
}
