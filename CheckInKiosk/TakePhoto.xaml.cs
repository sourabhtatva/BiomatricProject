using AForge.Video;
using AForge.Video.DirectShow;
using CheckInKiosk.Utils.Constants;
using CheckInKiosk.Utils.Models;
using CheckInKiosk.Utils.Resources.ApplicationData;
using CheckInKiosk.Utils.Services;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Point = System.Drawing.Point;


namespace CheckInKiosk
{
    public partial class TakePhoto : UserControl
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap capturedImage;
        private CancellationTokenSource cts;
        private DispatcherTimer captureTimer;
        private DispatcherTimer monitoringTimer;
        private bool isMonitoring = false;

        private bool faceDetected = false;
        private CascadeClassifier faceCascade;
        private HttpClientService _httpClientService;
        private CascadeClassifier eyeglassesCascade;
        private CascadeClassifier mouthCascade;
        private CascadeClassifier upperBodyCascade;
        private bool eyeglassesDetected = false;
        private bool maskDetected = false;
        private bool capDetected = false;

        public TakePhoto()
        {
            InitializeComponent();
            faceCascade = new CascadeClassifier(UIConstants.Haarcascade_Frontalface_Path);
            eyeglassesCascade = new CascadeClassifier(UIConstants.Haarcascade_Sunglasses_Path);
            mouthCascade = new CascadeClassifier(UIConstants.Haarcascade_Mask_Path);
            upperBodyCascade = new CascadeClassifier(UIConstants.Haarcascade_Upperbody_Path);

            captureTimer = new DispatcherTimer();
            captureTimer.Interval = TimeSpan.FromSeconds(50);
            captureTimer.Tick += CaptureTimer_Tick;

            monitoringTimer = new DispatcherTimer();
            monitoringTimer.Interval = TimeSpan.FromSeconds(10);
            monitoringTimer.Tick += MonitoringTimer_Tick;


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
        private void MonitoringTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer so it doesn't fire again before processing completes
            monitoringTimer.Stop();

            // Check the current states of object detection
            if (faceDetected)
            {
                if (!capDetected && !eyeglassesDetected && !maskDetected)
                {
                    isMonitoring = false;
                    monitoringTimer.Stop();
                    captureTimer.Start();
                    // All checks passed, no cap, eyeglasses, or mask detected
                    MessageBox.Show("Face detected successfully without cap, sunglasses, or mask.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Display the details of what's been detected
                    string message = "Face detected, but issues found:\n";

                    if (capDetected)
                        message += "- Cap detected\n";
                    if (eyeglassesDetected)
                        message += "- Eyeglasses detected\n";
                    if (maskDetected)
                        message += "- Mask not detected\n"; // Mask condition: Mouth detected -> no mask

                    MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                // No face detected
                MessageBox.Show("No face detected. Please adjust your position.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Reset the detection states for the next monitoring cycle
            faceDetected = false;
            eyeglassesDetected = false;
            maskDetected = false;
            capDetected = false;

            // Optionally, start the timer again if you want continuous monitoring
            // monitoringTimer.Start();
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
                    MessageBox.Show(UIMessages.FaceVerification.CameraNotFoundMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(UIMessages.FaceVerification.CameraStartErrorMessage(ex.Message));
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
                if (videoSource == null || !videoSource.IsRunning || cts?.Token.IsCancellationRequested == true)
                {
                    return;
                }

                capturedImage = (Bitmap)eventArgs.Frame.Clone();

                using (var imageFrame = capturedImage.ToImage<Bgr, byte>())
                {
                    var grayFrame = imageFrame.Convert<Gray, byte>();

                    // Detect faces
                    var faces = faceCascade.DetectMultiScale(grayFrame, 1.1, 10, System.Drawing.Size.Empty);
                    foreach (var face in faces)
                    {
                        imageFrame.Draw(face, new Bgr(Color.Red), 2);
                    }

                    // Detect eyeglasses
                    var eyeglasses = eyeglassesCascade.DetectMultiScale(grayFrame, 1.1, 10, new System.Drawing.Size(30, 30));
                    eyeglassesDetected = eyeglasses.Length > 0;
                    foreach (var glass in eyeglasses)
                    {
                        imageFrame.Draw(glass, new Bgr(Color.Green), 2);
                        CvInvoke.PutText(imageFrame, "Eyeglasses Detected", new Point(glass.X, glass.Y - 10), FontFace.HersheySimplex, 0.9, new MCvScalar(0, 255, 0), 2);
                    }

                    // Detect mouth (for mask detection)
                    var mouths = mouthCascade.DetectMultiScale(grayFrame, 1.1, 10, System.Drawing.Size.Empty);
                    maskDetected = mouths.Length == 0;  // No mouth detected -> mask on
                    foreach (var mouth in mouths)
                    {
                        imageFrame.Draw(mouth, new Bgr(Color.Yellow), 2);
                        CvInvoke.PutText(imageFrame, "Mouth Detected (No Mask)", new Point(mouth.X, mouth.Y - 10), FontFace.HersheySimplex, 0.9, new MCvScalar(0, 255, 255), 2);
                    }

                    // Detect upper body (used for cap detection)
                    var upperBodies = upperBodyCascade.DetectMultiScale(grayFrame, 1.1, 10, System.Drawing.Size.Empty);
                    capDetected = upperBodies.Length > 0;
                    foreach (var body in upperBodies)
                    {
                        imageFrame.Draw(body, new Bgr(Color.Blue), 2);
                        CvInvoke.PutText(imageFrame, "Cap Detected", new Point(body.X, body.Y - 10), FontFace.HersheySimplex, 0.9, new MCvScalar(255, 0, 0), 2);
                    }

                    // Update the webcam feed with the processed frame
                    WebcamFeed.Dispatcher.Invoke(() =>
                    {
                        WebcamFeed.Source = BitmapToImageSource(imageFrame.ToBitmap());
                    });

                    // Check monitoring conditions
                    if (faces.Length > 0 && !capDetected && !eyeglassesDetected && !maskDetected)
                    {
                        if (!faceDetected)
                        {
                            faceDetected = true;
                            if (!isMonitoring)
                            {
                                isMonitoring = true;
                                monitoringTimer.Start(); // Start monitoring if face is detected and no items are present
                            }
                        }
                    }
                    else
                    {
                        faceDetected = false;
                    }
                }
            }
            catch
            {
                // Handle exception silently
            }
        }

        private static BitmapSource BitmapToImageSource(Bitmap bitmap)
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

        private void CaptureAndVerify()
        {
            // Hide the WebcamFeed and other UI elements
            WebcamFeed.Visibility = Visibility.Collapsed;

            // Show Verification Message and Loading Indicator
            VerificationMessage.Text = UIMessages.FaceVerification.IdentityVerificationInProgressMessage;
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
                    var verifyImageResponse = await VerifyImageAsync(encryptedImageBase64String, documentScannedImageBase64String);

                    Dispatcher.Invoke(() =>
                    {
                        if (verifyImageResponse.IsIdentical)
                        {
                            VerificationMessage.Visibility = Visibility.Collapsed;
                            LoadingOverlay.Visibility = Visibility.Collapsed;
                            CheckInComplete.Visibility = Visibility.Visible;
                        }
                        else if(verifyImageResponse.ApiFailedStatus)
                        {
                            VerificationMessage.Visibility = Visibility.Collapsed;
                            LoadingOverlay.Visibility = Visibility.Collapsed;
                            CheckInComplete.Visibility = Visibility.Collapsed;
                            ApiFailedCase.Visibility = Visibility.Visible;
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
                        MessageBox.Show(UIMessages.FaceVerification.VerificationCancelMessage);
                        LoadingOverlay.Visibility = Visibility.Collapsed;
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(UIMessages.FaceVerification.VerificationErrorMessage(ex.Message));
                        LoadingOverlay.Visibility = Visibility.Collapsed;
                    });
                }
            }, token); // Pass the cancellation token here
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

                // Use the custom HttpClientService to send the POST request
                var responseData = await _httpClientService.PostAsync(APIEndpoint.FACE_MATCHING_API, jsonContent);

                // Parse the response data to extract the 'data' field
                var jsonDocument = JsonDocument.Parse(responseData);
                var data = jsonDocument.RootElement.GetProperty("data");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                response = JsonSerializer.Deserialize<FaceVerifyResponseUI>(data, options);

                return response;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(UIMessages.FaceVerification.VerificationApiErrorMessage(ex.Message));
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
