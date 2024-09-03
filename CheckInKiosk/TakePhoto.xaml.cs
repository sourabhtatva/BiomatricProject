using AForge.Video;
using AForge.Video.DirectShow;
using Azure.Core;
using BiometricAuthenticationAPI.Data.Models;
using CheckInKiosk.Utils.Constants;
using CheckInKiosk.Utils.Services;
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

namespace CheckInKiosk
{
    public partial class TakePhoto : UserControl
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap capturedImage;
        private CancellationTokenSource cts; // For cancelling ongoing tasks
        private HttpClientService _httpClientService;

        public event Action OnPhotoCaptured;

        public TakePhoto()
        {
            InitializeComponent();
        }

        // Constructor with HttpClientService for manual instantiation
        public TakePhoto(HttpClientService httpClientService) : this()
        {
            _httpClientService = httpClientService;
        }

        // Method to set the HttpClientService after instantiation
        public void SetHttpClientService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
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
                    // Log or handle exception as necessary
                }
                finally
                {
                    videoSource = null; // Ensure the videoSource is set to null
                }
            }
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

                // Update the ImagePreview on the UI thread
                ImagePreview.Dispatcher.Invoke(() =>
                {
                    ImagePreview.Source = BitmapToImageSource(capturedImage);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing frame: {ex.Message}");
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }


        private async void OnCaptureClick(object sender, RoutedEventArgs e)
        {
            if (capturedImage == null)
            {
                MessageBox.Show("No image captured.");
                return;
            }

            // Hide all elements except the Loading Indicator and Verification Message
            ImagePreview.Visibility = Visibility.Collapsed;
            ((Button)sender).Visibility = Visibility.Collapsed;

            // Show Verification Message and Loading Indicator
            VerificationMessage.Text = "We are verifying your identity. Please wait...";
            VerificationMessage.Visibility = Visibility.Visible;
            LoadingIndicator.Visibility = Visibility.Visible;

            // Add a delay to ensure UI updates are visible
            await Task.Delay(500); // Adjust the delay duration as needed

            // Convert captured image to byte array
            byte[] imageData = BitmapToByteArray(capturedImage);

            // Cancel any previous ongoing task
            cts?.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            try
            {
                ////For Demo

                //DisplayBoardingPass(null); // Pass null as we are using static data

                //// Simulate a delay for API verification
                //await Task.Delay(1000); // Simulate API processing time

                //// Hide Loading Indicator and show the static boarding pass
                //VerificationMessage.Visibility = Visibility.Collapsed;
                //LoadingIndicator.Visibility = Visibility.Collapsed;
                var request = new MatchFacesRequest()
                {
                    ScannedImage = imageData,
                    ClickedImage = imageData
                };

                 //var boardingPassDetails = await VerifyImageAsync(imageData, token);
                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    UIConstants.CONTENT_TYPE
                );
                var responseData = await _httpClientService.PostAsync(APIEndpoint.FACE_MATCHING_API, jsonContent);
                // Parse the JSON response and extract the 'data' field as a boolean
                var jsonDocument = JsonDocument.Parse(responseData);
                var data = jsonDocument.RootElement.GetProperty("data");

                LoadingIndicator.Visibility = Visibility.Collapsed;

                //if (boardingPassDetails != null)
                //{
                //    DisplayBoardingPass(boardingPassDetails);
                //    OnPhotoCaptured?.Invoke(); // Invoke the OnPhotoCaptured event
                //}
                //else
                //{
                //    VerificationMessage.Text = "Verification failed. Please try again.";
                //}
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show("The verification process was canceled.");
                LoadingIndicator.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during verification: {ex.Message}");
                LoadingIndicator.Visibility = Visibility.Collapsed;
            }
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                return memoryStream.ToArray();
            }
        }

        //private async Task<string> VerifyImageAsync(byte[] imageData, CancellationToken token)
        //{
        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            // Replace with your API endpoint
        //            string apiUrl = "https://your-api-endpoint.com/verify";

        //            // Send the image to the API
        //            HttpContent content = new ByteArrayContent(imageData);
        //            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        //            HttpResponseMessage response = await client.PostAsync(apiUrl, content, token);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                // Assuming the API returns the boarding pass details as a string
        //                return await response.Content.ReadAsStringAsync();
        //            }
        //            else
        //            {
        //                MessageBox.Show("Failed to verify the image.");
        //                return null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error calling API: {ex.Message}");
        //        return null;
        //    }
        //}

        private void DisplayBoardingPass(string details)
        {
            // Static demo boarding pass details
            string demoBoardingPass = "Boarding Pass\n" +
                "Passenger Name: John Doe\n" +
                "Flight Number: XY123\n" +
                "Departure: New York (JFK)\n" +
                "Arrival: Los Angeles (LAX)\n" +
                "Gate: 22\n" +
                "Seat: 12A\n" +
                "Boarding Time: 10:45 AM\n" +
                "Thank you for flying with us!";

            BoardingPassPanel.Visibility = Visibility.Visible;
            VerificationMessage.Visibility = Visibility.Collapsed;
            BoardingPassDetails.Text = demoBoardingPass;
        }

        private void OnFinishClick(object sender, RoutedEventArgs e)
        {
            // Show a MessageBox with a message and an OK button
            MessageBoxResult result = MessageBox.Show("Check-in complete. Have a great flight!",
                                                      "Check-In Complete",
                                                      MessageBoxButton.OK,
                                                      MessageBoxImage.Information);

            // Check if the user clicked OK
            if (result == MessageBoxResult.OK)
            {
                // Close the application
                Application.Current.Shutdown();
            }
        }

        // Handle cleanup when the UserControl is unloaded
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StopCamera();
            cts?.Cancel(); // Cancel any ongoing tasks
            cts?.Dispose(); // Dispose of the cancellation token source
        }
    }
}
