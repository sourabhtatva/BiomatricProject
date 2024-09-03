using System.Windows;
using System.Windows.Controls;

namespace CheckInKiosk
{
    public partial class MatchAndEnroll : UserControl
    {
        public event Action OnMatchingStarted;

        public MatchAndEnroll()
        {
            InitializeComponent();
        }

        private void OnStartMatchingClick(object sender, RoutedEventArgs e)
        {
            // Simulate matching process
            ResultText.Text = "Matching completed successfully.";
            OnMatchingStarted?.Invoke();
        }
    }
}
