﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Services.Maps;
using Windows.Web.Http;
using System.Text.Json;
using CheckInKiosk.Utils.Models;
using CheckInKiosk.Utils.Services;
using CheckInKiosk.Utils.Constants;
using System.Windows.Media.Imaging;
using System.Drawing;
using CheckInKiosk.Utils.Resources.ApplicationData;
using System.Drawing.Imaging;
using System.IO;

namespace CheckInKiosk
{
    public partial class ScanDocument : UserControl
    {
        public event Action OnScanSuccess;
        public event Action OnRetry;
        private HttpClientService _httpClientService;

        private string _selectedImagePath;

        public ScanDocument()
        {
            InitializeComponent();
        }

        // Constructor with HttpClientService for manual instantiation
        public ScanDocument(HttpClientService httpClientService) : this()
        {
            _httpClientService = httpClientService;
        }

        // Method to set the HttpClientService after instantiation
        public void SetHttpClientService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        private void OnDocumentTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Show the text field when a document type is selected
            AdditionalInfoTextBox.Visibility = Visibility.Visible;
            ImageUploadPanel.Visibility = Visibility.Visible;
        }

        private void OnChooseImageClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                UploadedImage.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
        }

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
                string documentScannedImagebase64Image = string.Empty;

                if (!string.IsNullOrEmpty(_selectedImagePath))
                {
                    Bitmap bitmap = new Bitmap(_selectedImagePath);
                    documentScannedImagebase64Image = BitmapToBase64String(bitmap);
                }

                // Store Base64 image data
                ApplicationData.DocumentScannedImage = documentScannedImagebase64Image;

                // Create the request object
                var request = new DocumentDetailRequestUI()
                {
                    DocumentNumber = additionalInfo,
                    DocumentType = documentType,
                    DocumentImage = documentScannedImagebase64Image
                };

                // Serialize request object to JSON
                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    UIConstants.CONTENT_TYPE
                );
                var responseData = await _httpClientService.PostAsync(APIEndpoint.VALIDATE_DOC_API, jsonContent);
                // Parse the JSON response and extract the 'data' field as a boolean
                var jsonDocument = JsonDocument.Parse(responseData);
                var data = jsonDocument.RootElement.GetProperty("data");
                bool isVerified = data.GetProperty("isValid").GetBoolean();

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

        private void OnOkayClick(object sender, RoutedEventArgs e)
        {
            // Close the application when the "Okay" button is clicked
            Application.Current.Shutdown();
        }

        private string BitmapToBase64String(Bitmap bitmapcapturedImage)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmapcapturedImage.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }


    }
}
