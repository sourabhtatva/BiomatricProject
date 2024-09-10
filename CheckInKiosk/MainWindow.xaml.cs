using CheckInKiosk.Utils.Services;
using System.Windows;

namespace CheckInKiosk
{
    public partial class MainWindow : Window
    {
        private readonly HttpClientService _httpClientService;

        public MainWindow()
        {
            InitializeComponent();

            _httpClientService = new HttpClientService();

            // Set the HttpClientService after instantiation
            scanDocument.SetHttpClientService(_httpClientService);
            takePhoto.SetHttpClientService(_httpClientService);
            biometricAppPopup.OnConsentYes += HandleConsentYes;
            biometricAppPopup.OnConsentNo += HandleConsentNo;
            scanDocument.OnScanSuccess += ShowTakePhoto;
            scanDocument.OnRetry += ShowRetryScan;
        }

        private void HandleConsentYes()
        {
            // Proceed to the ScanDocument screen
            biometricAppPopup.Visibility = Visibility.Collapsed;
            scanDocument.Visibility = Visibility.Visible;
        }

        private void HandleConsentNo()
        {
            // Show manual check-in message
            ManualCheckInMessage.Visibility = Visibility.Visible;
        }

        private void ShowTakePhoto()
        {
            scanDocument.Visibility = Visibility.Collapsed;
            takePhoto.Visibility = Visibility.Visible;

            // Start the camera when TakePhoto is visible
            StartTakePhotoCamera();
        }

        private void ShowRetryScan()
        {
            // Optionally handle retry scenario
        }

        private void StartTakePhotoCamera()
        {
            // Ensure takePhoto is not null and start the camera
            if (takePhoto != null)
            {
                takePhoto.StartCamera();
            }
        }
        public void RestartApplication()
        {
            // Close the application
            Application.Current.Shutdown();

            // Restart the application
            System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.FriendlyName);
            Environment.Exit(0); // Ensure the current instance is exited
        }

        private void StopTakePhotoCamera()
        {
            // Ensure takePhoto is not null and stop the camera
            if (takePhoto != null)
            {
                takePhoto.StopCamera();
            }
        }
    }
}
