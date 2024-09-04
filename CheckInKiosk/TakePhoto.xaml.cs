using AForge.Video;
using AForge.Video.DirectShow;
using BiometricAuthenticationAPI.Data.Models;
using CheckInKiosk.Utils.Constants;
using CheckInKiosk.Utils.Models;
using CheckInKiosk.Utils.Services;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Protection.PlayReady;

namespace CheckInKiosk
{
    public partial class TakePhoto : UserControl
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap capturedImage;
        private CancellationTokenSource cts;
        private HttpClientService _httpClientService;


        private DispatcherTimer captureTimer;
        private bool faceDetected = false;

        // Emgu CV objects
        private CascadeClassifier faceCascade;

        public event Action OnPhotoCaptured;

        public TakePhoto()
        {
            InitializeComponent();
            faceCascade = new CascadeClassifier(UIConstants.Haarcascade_Frontalface_Path);

            // Initialize the timer
            captureTimer = new DispatcherTimer();
            captureTimer.Interval = TimeSpan.FromSeconds(3); // Set delay for 10 seconds
            captureTimer.Tick += CaptureTimer_Tick;
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
                catch (Exception ex)
                {
                    MessageBox.Show($"Error stopping camera: {ex.Message}");
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
                if (videoSource == null || !videoSource.IsRunning)
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing frame: {ex.Message}");
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
            LoadingIndicator.Visibility = Visibility.Visible;

            // Convert captured image to byte array
            byte[] imageData = BitmapToByteArray(capturedImage);

            // Cancel any previous ongoing task
            cts?.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    var verificationSuccess = await VerifyImageAsync(imageData);

                    Dispatcher.Invoke(() =>
                    {
                        if (verificationSuccess)
                        {
                            // Show success message
                            MessageBoxResult result = MessageBox.Show("Check-in complete. Have a great flight!", "Check-In Complete", MessageBoxButton.OK, MessageBoxImage.Information);

                            if (result == MessageBoxResult.OK)
                            {
                                StopCamera();
                                Application.Current.Shutdown();
                            }
                        }
                        else
                        {
                            // Show error message
                            ManualCheckInPanel.Visibility = Visibility.Visible;
                        }
                    });
                }
                catch (TaskCanceledException)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("The verification process was canceled.");
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error during verification: {ex.Message}");
                    });
                }
            });
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                return memoryStream.ToArray();
            }
        }

        private async Task<bool> VerifyImageAsync(byte[] imageData)
        {
            try
            {
                var request = new MatchFacesUI()
                {
                    ScannedImage = imageData,
                    ClickedImage = imageData
                };

                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(request),
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
                var data = jsonDocument.RootElement.GetProperty("data").GetBoolean();

                return data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calling API: {ex.Message}");
                return false;
            }
            finally
            {
                // Always hide the loading indicator after the API call completes
                Dispatcher.Invoke(() => LoadingIndicator.Visibility = Visibility.Collapsed);
            }
        }

        private void OnOkayClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StopCamera();
            cts?.Cancel(); // Cancel any ongoing tasks
            cts?.Dispose(); // Dispose of the cancellation token source
        }
    }
}
