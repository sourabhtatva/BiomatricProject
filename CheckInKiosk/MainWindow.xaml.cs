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

            _httpClientService = new HttpClientService(); // Or inject it if using a DI container

            // Set the HttpClientService after instantiation
            scanDocument.SetHttpClientService(_httpClientService);
            takePhoto.SetHttpClientService(_httpClientService);
            biometricAppPopup.OnConsentYes += HandleConsentYes;
            biometricAppPopup.OnConsentNo += HandleConsentNo;
            scanDocument.OnScanSuccess += ShowTakePhoto;
            scanDocument.OnRetry += ShowRetryScan;

            // Ensure takePhoto is properly initialized and events are hooked up
            takePhoto.OnPhotoCaptured += ShowCompletion;
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

        private void ShowScanDocument()
        {
            biometricAppPopup.Visibility = Visibility.Collapsed;
            scanDocument.Visibility = Visibility.Visible;
        }

        private void ShowTakePhoto()
        {
            scanDocument.Visibility = Visibility.Collapsed;
            takePhoto.Visibility = Visibility.Visible;

            // Start the camera when TakePhoto is visible
            StartTakePhotoCamera();
        }

        private void ShowCompletion()
        {
            takePhoto.Visibility = Visibility.Collapsed;

            // Stop the camera when TakePhoto is no longer visible
            StopTakePhotoCamera();

            // Optionally handle final actions or show a completion message
        }

        private void ShowRetryScan()
        {
            // Optionally handle retry scenario
        }

        private void ShowManualCheckIn()
        {
            // Optionally handle manual check-in scenario
        }

        private void StartTakePhotoCamera()
        {
            // Ensure takePhoto is not null and start the camera
            if (takePhoto != null)
            {
                takePhoto.StartCamera();
            }
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
