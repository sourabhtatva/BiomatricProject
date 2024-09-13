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
using WIA;
using Microsoft.AspNetCore.Http;
using Tesseract;
using System.Text.RegularExpressions;
using System.Web;
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

        //private void OnAdditionalInfoTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(AdditionalInfoTextBox.Text))
        //    {
        //        PlaceholderTextBlock.Visibility = Visibility.Visible;
        //        AdditionalInfoErrorTextBlock.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        PlaceholderTextBlock.Visibility = Visibility.Collapsed;
        //        AdditionalInfoErrorTextBlock.Visibility = Visibility.Collapsed;
        //    }
        //}

        //private void OnChooseImageClick(object sender, RoutedEventArgs e)
        //{
        //    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
        //    {
        //        Filter = UIConstants.Filter
        //    };

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        _selectedImagePath = openFileDialog.FileName;
        //        UploadedImage.Source = new BitmapImage(new Uri(_selectedImagePath));
        //        UploadedImage.Visibility = Visibility.Visible;
        //        ImageUploadErrorTextBlock.Visibility = Visibility.Collapsed;

        //    }
        //}


        //private async void OnNextClick(object sender, RoutedEventArgs e)
        //{
        //    ScanDocumentPassport();
        //}

        //private void ScanDocumentPassport()
        //{
        //    try
        //    {
        //        var dialog = new CommonDialog();
        //        var scanner = dialog.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, false, false);

        //        if (scanner != null)
        //        {
        //            var scanItem = scanner.Items[1];
        //            var imageFile = (ImageFile)scanItem.Transfer(FormatID.wiaFormatPNG);
        //            byte[] bytes = (byte[])imageFile.FileData.get_BinaryData();                   

        //            // Extract text from the uploaded image using OCR
        //            string extractedText = ExtractTextFromImage(bytes);

        //            // Parse the passport details from the extracted text
        //            string passportNumber = ParsePassportDetails(extractedText);

        //            BitmapImage bitmap = new BitmapImage();
        //            using (MemoryStream ms = new MemoryStream(bytes))
        //            {
        //                ms.Position = 0;
        //                bitmap.BeginInit();
        //                bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
        //                bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //                bitmap.StreamSource = ms;
        //                bitmap.EndInit();
        //                bitmap.Freeze();  // To make the BitmapImage thread-safe
        //            }
        //            UploadedPreviewImage.Source = bitmap;
        //            UploadedPreviewImage.Visibility = Visibility.Visible;

        //            MainStackPanel.HorizontalAlignment = HorizontalAlignment.Left;
        //            MainStackPanel.Margin = new Thickness(50, 0, 0, 0);

        //            ImagePreviewStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
        //            ImagePreviewStackPanel.Margin = new Thickness(0,0,50,0);
        //            ImageUploadErrorTextBlock.Visibility = Visibility.Collapsed;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error: {ex.Message}");
        //    }
        //}

        public string ExtractTextFromImage(byte[] imageData)
        {
            try
            {
                // Path to tessdata folder containing language data
                string tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "eng.traineddata");

                if (!File.Exists(tessDataPath))
                {
                    throw new FileNotFoundException("Tesseract language file not found.");
                }

                // Initialize Tesseract OCR Engine
                using (var ocrEngine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"), "eng", EngineMode.Default))
                {
                    // Load the image from file
                    using (var img = Pix.LoadFromMemory(imageData))
                    {
                        // Process the image and extract text
                        using (var page = ocrEngine.Process(img))
                        {
                            return page.GetText();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or display the error                
                LoadingOverlay.Visibility = Visibility.Collapsed;
                VerificationMessage.Text = UIMessages.DocumentVerification.DocVerificationErrorMessage(ex.Message);
                return string.Empty;
            }
        }

        public string ParsePassportDetails(string ocrText)
        {
            string passportNumberPattern = @"\n([A-Z]\d{7})";
            // string veitnamIdNumberPattern = @"[0-9]{9}";


            var passportNumberMatch = Regex.Match(ocrText, passportNumberPattern);
            // var veitnamIdNumberMatch = Regex.Match(ocrText, veitnamIdNumberPattern);
            string number = string.Empty;


            if (passportNumberMatch.Success)
            {
                number = passportNumberMatch.Value.Replace("\n", string.Empty).Trim();
            }

            //if (veitnamIdNumberMatch.Success)
            //{
            //    number = veitnamIdNumberMatch.Value;
            //}

            return number;
        }


        private async void OnNextClick(object sender, RoutedEventArgs e)
        {
            // Get selected document type
            string documentType = (DocumentTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
            string documentScannedImage = ApplicationData.DocumentScannedImage;
            string passportNumber = ApplicationData.PassportNumber;
            bool hasError = false;

            // Check if document type is selected
            if (string.IsNullOrEmpty(documentType))
            {
                DocumentTypeErrorTextBlock.Visibility = Visibility.Visible;
                hasError = true;
            }            

            if (hasError)
            {
                return; // Exit if there are validation errors
            }

            // Hide all UI elements except Loading Indicator and Verification Message
            MainStackPanel.Visibility = Visibility.Collapsed;
            ImagePreviewStackPanel.Visibility = Visibility.Collapsed;
            LoadingOverlay.Visibility = Visibility.Visible;
            VerificationMessage.Visibility = Visibility.Visible;

            // Set a default message for VerificationMessage
            VerificationMessage.Text = UIMessages.DocumentVerification.DocVerificationInProgressMessage(documentType);

            // Ensure UI updates are applied before starting the verification process
            //await Task.Delay(500); // Delay to show the loader and message

            try
            {                                             
                // Encrypt document type and additional info
                string encryptedDocumentType = Encryptor.EncryptString(documentType);
                string encryptedPassportNumber = Encryptor.EncryptString(passportNumber);

                byte[] clickedScannedImageData = Convert.FromBase64String(documentScannedImage);
                byte[] encryptedScannedImageData = Encryptor.EncryptByteArray(clickedScannedImageData);
                string encryptedScannedImageBase64String = Convert.ToBase64String(encryptedScannedImageData);


                var request = new DocumentDetailRequestUI()
                {
                    DocumentType = encryptedDocumentType,
                    DocumentNumber = encryptedPassportNumber,
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
        private async void OnScanClick(object sender, RoutedEventArgs e)
        {
            // Hide all UI elements except Loading Indicator and Verification Message
            MainStackPanel.Visibility = Visibility.Collapsed;
            LoadingOverlay.Visibility = Visibility.Visible;
            VerificationMessage.Visibility = Visibility.Visible;
            // Get selected document type
            string documentType = (DocumentTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;            

            bool hasError = false;

            // Check if document type is selected
            if (string.IsNullOrEmpty(documentType))
            {
                DocumentTypeErrorTextBlock.Visibility = Visibility.Visible;
                hasError = true;
            }          

            if (hasError)
            {
                return; // Exit if there are validation errors
            }
                      
            // Set a default message for VerificationMessage
            VerificationMessage.Text = UIMessages.DocumentVerification.DocVerificationInProgressMessage(documentType);
            //await Task.Delay(500); // Delay to show the loader and message
            try
            {
                string documentScannedImagebase64Image = string.Empty;                

                var dialog = new CommonDialog();
                var scanner = dialog.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, false, false);
                string passportNumber = string.Empty;
                if (scanner != null)
                {
                    var scanItem = scanner.Items[1];
                    var imageFile = (ImageFile)scanItem.Transfer(FormatID.wiaFormatPNG);
                    byte[] bytes = (byte[])imageFile.FileData.get_BinaryData();
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        Bitmap bitmap = new Bitmap(ms);
                        ApplicationData.DocumentScannedImage = BitmapToBase64String(bitmap);
                    }

                    // Extract text from the uploaded image using OCR
                    string extractedText = ExtractTextFromImage(bytes);

                    // Parse the passport details from the extracted text
                    passportNumber = ParsePassportDetails(extractedText);
                    ApplicationData.PassportNumber = passportNumber;

                    BitmapImage bitmapImage = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        ms.Position = 0;
                        bitmapImage.BeginInit();
                        bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = ms;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();  // To make the BitmapImage thread-safe
                    }
                    LoadingOverlay.Visibility = Visibility.Collapsed;
                    VerificationMessage.Visibility = Visibility.Collapsed;
                    UploadedPreviewImage.Source = bitmapImage;                    
                    ImagePreviewStackPanel.Visibility = Visibility.Visible;
                    UploadedPreviewImage.Visibility = Visibility.Visible;
                    ImageUploadErrorTextBlock.Visibility = Visibility.Collapsed;
                    NextPreviewButton.Visibility = Visibility.Visible;
                }                              
            }
            catch (Exception ex)
            {
                // Hide loading indicator and show error message
                LoadingOverlay.Visibility = Visibility.Collapsed;
                VerificationMessage.Text = UIMessages.DocumentVerification.DocVerificationErrorMessage(ex.Message);
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
                bitmapcapturedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
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
