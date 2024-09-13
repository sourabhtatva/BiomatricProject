using CheckInKiosk.Utils.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CheckInKiosk
{
    public partial class MainWindow : Window
    {
        private readonly HttpClientService _httpClientService;
        private DispatcherTimer _inactivityTimer;
        private TimeSpan _inactivityDuration = TimeSpan.FromSeconds(15); // Set to 15 seconds
        private TimeSpan _remainingTime;

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

            // Initialize the inactivity timer
            InitializeInactivityTimer();

            // Register event handlers for user actions
            this.MouseMove += UserActivityDetected;
            this.KeyDown += UserActivityDetected;
            this.MouseDown += UserActivityDetected;
        }

        private void InitializeInactivityTimer()
        {
            _inactivityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _inactivityTimer.Tick += OnInactivityTimerTick;
        }

        private void ResetInactivityTimer()
        {
            _remainingTime = _inactivityDuration;
            CountdownTimerText.Text = _remainingTime.ToString(@"ss");
            _inactivityTimer.Stop();
            _inactivityTimer.Start();
        }

        private void OnInactivityTimerTick(object sender, EventArgs e)
        {
            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
            CountdownTimerText.Text = _remainingTime.ToString(@"ss");

            if (_remainingTime <= TimeSpan.Zero)
            {
                _inactivityTimer.Stop();
                RedirectToMainScreen();
            }
        }

        private void UserActivityDetected(object sender, EventArgs e)
        {
            ResetInactivityTimer();
        }

        private void RedirectToMainScreen()
        {
            biometricAppPopup.Visibility = Visibility.Visible;
            scanDocument.Visibility = Visibility.Collapsed;
            takePhoto.Visibility = Visibility.Collapsed;

            StopTakePhotoCamera();
            CountdownTimerText.Text = string.Empty; // Clear the countdown display
        }

        private void HandleConsentYes()
        {
            biometricAppPopup.Visibility = Visibility.Collapsed;
            scanDocument.Visibility = Visibility.Visible;
            ResetInactivityTimer();
        }

        private void HandleConsentNo()
        {
            ManualCheckInMessage.Visibility = Visibility.Visible;
            ResetInactivityTimer();
        }

        private void ShowTakePhoto()
        {
            scanDocument.Visibility = Visibility.Collapsed;
            takePhoto.Visibility = Visibility.Visible;
            StartTakePhotoCamera();
            ResetInactivityTimer();
        }

        private void ShowRetryScan()
        {
            ResetInactivityTimer();
        }

        private void StartTakePhotoCamera()
        {
            takePhoto?.StartCamera();
            ResetInactivityTimer();
        }

        private void StopTakePhotoCamera()
        {
            takePhoto?.StopCamera();
        }

        public void RestartApplication()
        {
            // Close the application
            Application.Current.Shutdown();

            // Restart the application
            System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.FriendlyName);
            Environment.Exit(0); // Ensure the current instance is exited
        }
    }
}
