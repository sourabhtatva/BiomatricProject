using CheckInKiosk.Utils.Services;
using System;
using System.Windows;
using System.Windows.Threading;

namespace CheckInKiosk
{
    public partial class MainWindow : Window
    {
        private readonly HttpClientService _httpClientService;
        private DispatcherTimer _timeoutTimer;
        private DispatcherTimer _countdownTimer;  // Timer for updating countdown text
        private TimeSpan _timeoutDuration = TimeSpan.FromMinutes(2); // Set the timeout duration (5 minutes)
        private TimeSpan _remainingTime; // To track the remaining time

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

            // Initialize the timers
            InitializeTimeoutTimer();
        }

        private void InitializeTimeoutTimer()
        {
            // Timeout timer (fires once after timeout duration)
            _timeoutTimer = new DispatcherTimer();
            _timeoutTimer.Interval = _timeoutDuration;
            _timeoutTimer.Tick += OnTimeout;

            // Countdown timer (updates every second)
            _countdownTimer = new DispatcherTimer();
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);
            _countdownTimer.Tick += UpdateCountdown;
        }

        private void ResetTimeoutTimer()
        {
            _timeoutTimer.Stop();
            _countdownTimer.Stop();

            _remainingTime = _timeoutDuration; // Reset the remaining time

            _timeoutTimer.Start();
            _countdownTimer.Start();
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            _timeoutTimer.Stop();
            _countdownTimer.Stop();
            RedirectToMainScreen();
        }

        private void UpdateCountdown(object sender, EventArgs e)
        {
            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
            CountdownTimerText.Text = _remainingTime.ToString(@"mm\:ss"); // Display minutes and seconds

            if (_remainingTime <= TimeSpan.Zero)
            {
                // Stop the countdown if time runs out (this is just a safeguard)
                _countdownTimer.Stop();
            }
        }

        private void RedirectToMainScreen()
        {
            biometricAppPopup.Visibility = Visibility.Visible;
            scanDocument.Visibility = Visibility.Collapsed;
            takePhoto.Visibility = Visibility.Collapsed;

            StopTakePhotoCamera();

            // Reset the countdown display
            CountdownTimerText.Text = string.Empty;
        }

        private void HandleConsentYes()
        {
            biometricAppPopup.Visibility = Visibility.Collapsed;
            scanDocument.Visibility = Visibility.Visible;
            ResetTimeoutTimer();
        }

        private void HandleConsentNo()
        {
            ManualCheckInMessage.Visibility = Visibility.Visible;
            ResetTimeoutTimer();
        }

        private void ShowTakePhoto()
        {
            scanDocument.Visibility = Visibility.Collapsed;
            takePhoto.Visibility = Visibility.Visible;
            StartTakePhotoCamera();
            ResetTimeoutTimer();
        }

        private void ShowRetryScan()
        {
            ResetTimeoutTimer();
        }

        private void StartTakePhotoCamera()
        {
            takePhoto?.StartCamera();
            ResetTimeoutTimer();
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
