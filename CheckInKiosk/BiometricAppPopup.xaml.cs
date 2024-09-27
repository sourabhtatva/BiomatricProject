using System;
using System.Windows;
using System.Windows.Controls;

namespace CheckInKiosk
{
    public partial class BiometricAppPopup : UserControl
    {
        public event Action OnConsentYes;
        public event Action OnConsentNo;

        public BiometricAppPopup()
        {
            InitializeComponent();
        }

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

        private void ShowError(Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
