using CheckInKiosk.Utils.Constants;
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
        private TimeSpan _inactivityDuration = TimeSpan.FromSeconds(UIConstants.InactivityDuration);
        private TimeSpan _remainingTime;
        private bool _isMainScreenActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
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

        /// <summary>
        /// Initializes the inactivity timer.
        /// </summary>
        private void InitializeInactivityTimer()
        {
            _inactivityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(UIConstants.TimeIntervalForOneSecond)
            };
            _inactivityTimer.Tick += OnInactivityTimerTick;
        }

        /// <summary>
        /// Resets the inactivity timer.
        /// </summary>
        private void ResetInactivityTimer()
        {
            if (!_isMainScreenActive)
            {
                _remainingTime = _inactivityDuration;
                _inactivityTimer.Stop();
                _inactivityTimer.Start();
            }
        }

        /// <summary>
        /// Called when inactivity timer tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnInactivityTimerTick(object sender, EventArgs e)
        {
            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(UIConstants.TimeIntervalForOneSecond));

            if (_remainingTime <= TimeSpan.Zero)
            {
                _inactivityTimer.Stop();
                RestartApplication();
            }
        }

        /// <summary>
        /// Users the activity detected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void UserActivityDetected(object sender, EventArgs e)
        {
            ResetInactivityTimer();
        }

        /// <summary>
        /// Redirects to main screen.
        /// </summary>
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

        /// <summary>
        /// Handles the consent yes for privacy policy.
        /// </summary>
        private void HandleConsentYes()
        {
            biometricAppPopup.Visibility = Visibility.Collapsed;
            scanDocument.Visibility = Visibility.Visible;
            ResetInactivityTimer();
            SetMainScreenInactive();
        }

        /// <summary>
        /// Handles the consent no for privacy policy.
        /// </summary>
        private void HandleConsentNo()
        {
            ManualCheckInMessage.Visibility = Visibility.Visible;
            ResetInactivityTimer();
            SetMainScreenInactive();
        }

        /// <summary>
        /// Shows the take photo.
        /// </summary>
        private void ShowTakePhoto()
        {
            scanDocument.Visibility = Visibility.Collapsed;
            takePhoto.Visibility = Visibility.Visible;
            StartTakePhotoCamera();
            ResetInactivityTimer();
            SetMainScreenInactive();
        }

        /// <summary>
        /// Shows the retry scan.
        /// </summary>
        private void ShowRetryScan()
        {
            ResetInactivityTimer();
        }

        /// <summary>
        /// Starts the camera to take photo.
        /// </summary>
        private void StartTakePhotoCamera()
        {
            takePhoto?.StartCamera();
            ResetInactivityTimer();
        }

        /// <summary>
        /// Stops the camera.
        /// </summary>
        private void StopTakePhotoCamera()
        {
            takePhoto?.StopCamera();
        }

        private void SetMainScreenInactive()
        {
            _isMainScreenActive = false;
        }

        /// <summary>
        /// Restarts the application.
        /// </summary>
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
