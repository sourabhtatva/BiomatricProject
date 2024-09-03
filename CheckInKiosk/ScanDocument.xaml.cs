using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Services.Maps;
using Windows.Web.Http;
using System.Text.Json;

namespace CheckInKiosk
{
    public partial class ScanDocument : UserControl
    {
        public event Action OnScanSuccess;
        public event Action OnRetry;
        private readonly System.Net.Http.HttpClient _httpClient;


        public ScanDocument()
        {
            InitializeComponent();
            _httpClient = new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri("http://localhost:5062/")
            };
        }

        private void OnDocumentTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Show the text field when a document type is selected
            AdditionalInfoTextBox.Visibility = Visibility.Visible;
        }

        private static readonly Dictionary<(string DocumentType, string AdditionalInfo), bool> DocumentVerificationData = new()
        {
            { ("Passport", "123456789"), true },
            { ("Driver's License", "A1234567"), true },
            { ("ID Card", "ID987654"), false } // Example of a document that fails verification
        };

        private async void OnSubmitClick(object sender, RoutedEventArgs e)
        {
            // Get selected document type
            string documentType = (DocumentTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Unknown";
            string additionalInfo = AdditionalInfoTextBox.Text;

            // Hide all UI elements except Loading Indicator and Verification Message
            MainStackPanel.Visibility = Visibility.Collapsed;
            LoadingOverlay.Visibility = Visibility.Visible;
            VerificationMessage.Visibility = Visibility.Visible;

            // Set a default message for VerificationMessage
            VerificationMessage.Text = $"Verifying given details for {documentType}...";

            // Ensure UI updates are applied before starting the verification process
            await Task.Delay(500); // Delay to show the loader and message

            try
            {

                // Create the request object
                var request = new DocumentDetailRequest
                {
                    DocumentNumber = additionalInfo,
                    DocumentType = documentType
                };

                // Serialize request object to JSON
                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
                var response = await _httpClient.PostAsync("api/DocumentScan/validate", jsonContent);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                // Parse the JSON response and extract the 'data' field as a boolean
                var jsonDocument = JsonDocument.Parse(responseData);
                bool isVerified = jsonDocument.RootElement.GetProperty("data").GetBoolean();

                // Hide loading indicator
                LoadingOverlay.Visibility = Visibility.Collapsed;

                if (isVerified)
                {
                    VerificationMessage.Text = "Document verified successfully!";
                    OnScanSuccess?.Invoke();
                }
                else
                {
                    VerificationMessage.Visibility = Visibility.Collapsed;
                    ManualCheckInPanel.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                // Hide loading indicator and show error message
                LoadingOverlay.Visibility = Visibility.Collapsed;
                VerificationMessage.Text = $"An error occurred: {ex.Message}";
            }
        }


        private Task<bool> VerifyDocumentAsync(string documentType, string additionalInfo)
        {
            // Use static data for verification
            bool isVerified = DocumentVerificationData.TryGetValue((documentType, additionalInfo), out bool result) && result;
            return Task.FromResult(isVerified);
        }

        private void OnOkayClick(object sender, RoutedEventArgs e)
        {
            // Close the application when the "Okay" button is clicked
            Application.Current.Shutdown();
        }
    }
}
