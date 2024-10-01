using System;
using System.Windows;
using System.Windows.Controls;

namespace CheckInKiosk
{
    public partial class BiometricAppPopup : UserControl
    {
        public event Action OnConsentYes;
        public event Action OnConsentNo;

        /// <summary>
        /// Initializes a new instance of the <see cref="BiometricAppPopup"/> class.
        /// </summary>
        public BiometricAppPopup()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when [yes click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnYesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OnConsentYes?.Invoke();
                // Hide this control and proceed to the next step
                Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Called when [no click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnNoClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Hide the consent panel and show the manual check-in panel
                ConsentPanel.Visibility = Visibility.Collapsed;
                ManualCheckInPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Called when [okay click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnOkayClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Notify the MainWindow to restart the application
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.RestartApplication();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
       
        /// <summary>
        /// Shows the error using MessageBox.
        /// </summary>
        /// <param name="ex">The ex.</param>
        private void ShowError(Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
