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

        private bool _isMainScreenActive;

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

            // Initially set the main screen as active
            SetMainScreenActive();
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
            if (!_isMainScreenActive)
            {
                _remainingTime = _inactivityDuration;
                _inactivityTimer.Stop();
                _inactivityTimer.Start();
            }
        }

        private void OnInactivityTimerTick(object sender, EventArgs e)
        {
            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));

            if (_remainingTime <= TimeSpan.Zero)
            {
                _inactivityTimer.Stop();
                RestartApplication();
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
            SetMainScreenActive();
        }

        private void SetMainScreenActive()
        {
            _isMainScreenActive = true;
            _inactivityTimer.Stop();
        }

        private void HandleConsentYes()
        {
            biometricAppPopup.Visibility = Visibility.Collapsed;
            scanDocument.Visibility = Visibility.Visible;
            ResetInactivityTimer();
            SetMainScreenInactive();
        }

        private void HandleConsentNo()
        {
            ManualCheckInMessage.Visibility = Visibility.Visible;
            ResetInactivityTimer();
            SetMainScreenInactive();
        }

        private void ShowTakePhoto()
        {
            scanDocument.Visibility = Visibility.Collapsed;
            takePhoto.Visibility = Visibility.Visible;
            StartTakePhotoCamera();
            ResetInactivityTimer();
            SetMainScreenInactive();
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

        private void SetMainScreenInactive()
        {
            _isMainScreenActive = false;
        }

        public void RestartApplication()
        {
            StopTakePhotoCamera();

            string appPath = System.AppDomain.CurrentDomain.FriendlyName;

            var newProcess = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = appPath,
                    UseShellExecute = true
                }
            };

            Application.Current.Shutdown();
            newProcess.Start();
            Environment.Exit(0);
        }
    }
}
