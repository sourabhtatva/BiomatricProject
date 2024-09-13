using System;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Text.Json;
using CheckInKiosk.Utils.Models;
using CheckInKiosk.Utils.Services;
using CheckInKiosk.Utils.Constants;
using System.Windows.Media.Imaging;
using System.Drawing;
using CheckInKiosk.Utils.Resources.ApplicationData;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace CheckInKiosk
{
    public partial class ScanDocument : UserControl
    {
        public event Action OnScanSuccess;
        public event Action OnRetry;
        private HttpClientService _httpClientService;
        private string _selectedImagePath;
        private string _documentType;
        private Storyboard _loadingStoryboard;

        public ScanDocument()
        {
            InitializeComponent();
            InitializeLoaderAnimation();
        }

        // Constructor with HttpClientService for manual instantiation
        public ScanDocument(HttpClientService httpClientService) : this()
        {
            _httpClientService = httpClientService;
        }

        public void SetHttpClientService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        private void OnPassportButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _documentType = "Passport";
                OnDocumentTypeSelected();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error selecting passport: {ex.Message}");
            }
        }

        private void OnIDCardButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _documentType = "Vietnam ID";
                OnDocumentTypeSelected();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error selecting ID card: {ex.Message}");
            }
        }

        private void OnDocumentTypeSelected()
        {
            try
            {
                if (string.IsNullOrEmpty(_documentType))
                {
                    DocumentTypeErrorTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    PlaceholderTextBlock.Visibility = Visibility.Visible;
                    AdditionalInfoTextBox.Visibility = Visibility.Visible;
                    ImageUploadPanel.Visibility = Visibility.Visible;
                    DocumentTypeErrorTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error processing document type: {ex.Message}");
            }
        }

        private void OnAdditionalInfoTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AdditionalInfoTextBox.Text))
                {
                    PlaceholderTextBlock.Visibility = Visibility.Visible;
                    AdditionalInfoErrorTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    PlaceholderTextBlock.Visibility = Visibility.Collapsed;
                    AdditionalInfoErrorTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error processing additional information: {ex.Message}");
            }
        }

        private void OnChooseImageClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = UIConstants.Filter
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    _selectedImagePath = openFileDialog.FileName;
                    UploadedImage.Source = new BitmapImage(new Uri(_selectedImagePath));
                    UploadedImage.Visibility = Visibility.Visible;
                    ImageUploadErrorTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error choosing image: {ex.Message}");
            }
        }

        private async void OnSubmitClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string additionalInfo = AdditionalInfoTextBox.Text;
                bool hasError = false;

                if (string.IsNullOrEmpty(_documentType))
                {
                    DocumentTypeErrorTextBlock.Visibility = Visibility.Visible;
                    hasError = true;
                }

                if (string.IsNullOrEmpty(additionalInfo) && AdditionalInfoTextBox.Visibility == Visibility.Visible)
                {
                    AdditionalInfoErrorTextBlock.Visibility = Visibility.Visible;
                    hasError = true;
                }

                if (string.IsNullOrEmpty(_selectedImagePath) && ImageUploadPanel.Visibility == Visibility.Visible)
                {
                    ImageUploadErrorTextBlock.Visibility = Visibility.Visible;
                    hasError = true;
                }

                if (hasError)
                {
                    return;
                }

                MainStackPanel.Visibility = Visibility.Collapsed;
                ShowLoadingOverlay();
                VerificationMessage.Visibility = Visibility.Visible;
                VerificationMessage.Text = UIMessages.DocumentVerification.DocVerificationInProgressMessage(_documentType);

                await Task.Delay(500);

                string documentScannedImagebase64Image = string.Empty;

                if (!string.IsNullOrEmpty(_selectedImagePath))
                {
                    Bitmap bitmap = new Bitmap(_selectedImagePath);
                    documentScannedImagebase64Image = BitmapToBase64String(bitmap);
                }

                string encryptedDocumentType = Encryptor.EncryptString(_documentType);
                string encryptedAdditionalInfo = Encryptor.EncryptString(additionalInfo);

                byte[] clickedScannedImageData = Convert.FromBase64String(documentScannedImagebase64Image);
                byte[] encryptedScannedImageData = Encryptor.EncryptByteArray(clickedScannedImageData);
                string encryptedScannedImageBase64String = Convert.ToBase64String(encryptedScannedImageData);

                ApplicationData.DocumentScannedImage = encryptedScannedImageBase64String;

                var request = new DocumentDetailRequestUI()
                {
                    DocumentType = encryptedDocumentType,
                    DocumentNumber = encryptedAdditionalInfo,
                    DocumentImage = encryptedScannedImageBase64String
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    UIConstants.CONTENT_TYPE
                );
                var responseData = await _httpClientService.PostAsync(APIEndpoint.VALIDATE_DOC_API, jsonContent);
                var jsonDocument = JsonDocument.Parse(responseData);
                var data = jsonDocument.RootElement.GetProperty("data");
                bool isVerified = data.GetProperty("isValid").GetBoolean();

                HideLoadingOverlay();

                if (isVerified)
                {
                    VerificationMessage.Text = UIMessages.DocumentVerification.DocVerificationSuccessMessage;
                    OnScanSuccess?.Invoke();
                }
                else
                {
                    VerificationMessage.Visibility = Visibility.Collapsed;
                    ManualCheckInPanel.Visibility = Visibility.Visible;
                }
            }
            catch (HttpRequestException ex)
            {
                HideLoadingOverlay();
                ShowErrorMessage("Network error occurred during document verification.");
            }
            catch (JsonException ex)
            {
                HideLoadingOverlay();
                ShowErrorMessage("Error parsing server response.");
            }
            catch (Exception ex)
            {
                HideLoadingOverlay();
                ShowErrorMessage($"An unexpected error occurred: {ex.Message}");
            }
        }

        private void OnOkayClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.RestartApplication();
            }
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

        private void InitializeLoaderAnimation()
        {
            _loadingStoryboard = new Storyboard();
            var rotateAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(rotateAnimation, RotateTransform);
            Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath(RotateTransform.AngleProperty));
            _loadingStoryboard.Children.Add(rotateAnimation);
        }

        private void ShowLoadingOverlay()
        {
            LoadingOverlay.Visibility = Visibility.Visible;
            _loadingStoryboard.Begin();
        }

        private void HideLoadingOverlay()
        {
            LoadingOverlay.Visibility = Visibility.Collapsed;
            _loadingStoryboard.Stop();
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
