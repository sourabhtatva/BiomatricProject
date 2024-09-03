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
            OnConsentYes?.Invoke();
            // Hide this control and proceed to the next step
            Visibility = Visibility.Collapsed;
        }

        private void OnNoClick(object sender, RoutedEventArgs e)
        {
            // Hide the consent panel and show the manual check-in panel
            ConsentPanel.Visibility = Visibility.Collapsed;
            ManualCheckInPanel.Visibility = Visibility.Visible;
            OnConsentNo?.Invoke();

        }

        private void OnOkayClick(object sender, RoutedEventArgs e)
        {
            // Exit the application when the Okay button is clicked
            Application.Current.Shutdown();
        }
    }
}
