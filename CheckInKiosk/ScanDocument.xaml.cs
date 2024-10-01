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
using System.Threading.Tasks;
using System.Net;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Path = System.IO.Path;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanDocument"/> class.
        /// </summary>
        public ScanDocument()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanDocument"/> class.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service.</param>
        public ScanDocument(HttpClientService httpClientService) : this()
        {
            _httpClientService = httpClientService;
        }

        /// <summary>
        /// Sets the HTTP client service.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service.</param>
        public void SetHttpClientService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        /// <summary>
        /// Called when document option passport click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnPassportButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _documentType = UIConstants.DocumentTypePassport;
                PassportButton.Style = (Style)FindResource("HighlightButtonStyle");
                IDCardButton.Style = (Style)FindResource("DefaultButtonStyle");
                OnDocumentTypeSelected();
            }
            catch (Exception ex)
            {
                ManualCheckInPanel.Visibility = Visibility.Visible;
                ShowErrorMessage($"Error selecting passport: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when document option Vietnam ID click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnIDCardButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _documentType = UIConstants.DocumentTypeVietnamId;
                IDCardButton.Style = (Style)FindResource("HighlightButtonStyle");
                PassportButton.Style = (Style)FindResource("DefaultButtonStyle");
                OnDocumentTypeSelected();
            }
            catch (Exception ex)
            {
                ManualCheckInPanel.Visibility = Visibility.Visible;
                ShowErrorMessage($"Error selecting ID card: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when [document type selected].
        /// </summary>
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
                    ScannerImage.Visibility = Visibility.Visible;
                    ScanDocumentButton.Visibility = Visibility.Visible;
                    DocumentTypeErrorTextBlock.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                ManualCheckInPanel.Visibility = Visibility.Visible;
                ShowErrorMessage($"Error processing document type: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when [next click].
        /// </summary>
        private async void OnNextClick()
        {
            try
            {
                string documentScannedImage = ApplicationData.DocumentScannedImage;
                string documentNumber = ApplicationData.DocumentNumber;
                bool hasError = false;

                if (string.IsNullOrEmpty(_documentType))
                {
                    DocumentTypeErrorTextBlock.Visibility = Visibility.Visible;
                    hasError = true;
                }

                if (hasError)
                {
                    return; // Exit if there are validation errors
                }

                VerificationMessage.Text = UIMessages.DocumentVerification.DocVerificationInProgressMessage(_documentType);

                try
                {
                    var request = new DocumentDetailRequestUI()
                    {
                        DocumentType = Encryptor.EncryptString(_documentType),
                        DocumentNumber = Encryptor.EncryptString(documentNumber),
                        DocumentImage = Convert.ToBase64String(
                            Encryptor.EncryptByteArray(
                                Convert.FromBase64String(documentScannedImage)
                            )
                        )
                    };

                    var jsonContent = new StringContent(
                        JsonSerializer.Serialize(request),
                        System.Text.Encoding.UTF8,
                        UIConstants.CONTENT_TYPE
                    );

                    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(UIConstants.ResetTimeoutForCancellationToken)))
                    {
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
                }
                catch (OperationCanceledException)
                {
                    HideLoadingOverlay();
                    ManualCheckInPanel.Visibility = Visibility.Visible;
                    ShowErrorMessage("The request timed out. Please try again later.");
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    HideLoadingOverlay();
                    ManualCheckInPanel.Visibility = Visibility.Visible;
                    ShowErrorMessage("Request to server timed out. Please try again.");
                }
                catch (HttpRequestException ex)
                {
                    HideLoadingOverlay();
                    ManualCheckInPanel.Visibility = Visibility.Visible;
                    ShowErrorMessage("Network error occurred during document verification.");
                }
                catch (JsonException ex)
                {
                    HideLoadingOverlay();
                    ManualCheckInPanel.Visibility = Visibility.Visible;
                    ShowErrorMessage("Error parsing server response.");
                }
            }
            catch (Exception ex)
            {
                HideLoadingOverlay();
                VerificationMessage.Visibility = Visibility.Collapsed;
                ManualCheckInPanel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Called when [scan click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="InvalidOperationException">No scanner device found.</exception>
        private async void OnScanClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MainStackPanel.Visibility = Visibility.Collapsed;
                LoadingOverlay.Visibility = Visibility.Visible;
                VerificationMessage.Visibility = Visibility.Visible;

                bool hasError = false;

                if (string.IsNullOrEmpty(_documentType))
                {
                    DocumentTypeErrorTextBlock.Visibility = Visibility.Visible;
                    hasError = true;
                }

                if (hasError)
                {
                    return;
                }

                VerificationMessage.Text = UIMessages.DocumentVerification.DocVerificationInProgressMessage(_documentType);

                try
                {
                    var dialog = new CommonDialog();
                    var scanner = dialog.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, false, false);
                    if (scanner == null)
                    {
                        ManualCheckInPanel.Visibility = Visibility.Visible;
                        throw new InvalidOperationException("No scanner device found.");
                    }

                    var scanItem = scanner.Items[1];
                    var imageFile = (ImageFile)scanItem.Transfer(FormatID.wiaFormatPNG);
                    byte[] bytes = (byte[])imageFile.FileData.get_BinaryData();

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        Bitmap bitmap = new Bitmap(ms);
                        ApplicationData.DocumentScannedImage = BitmapToBase64String(bitmap);
                    }

                    string extractedText = ExtractTextFromImage(bytes);
                    string documentNumber = ParseDocumentDetails(extractedText);
                    ApplicationData.DocumentNumber = documentNumber;

                    BitmapImage bitmapImage = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        ms.Position = 0;
                        bitmapImage.BeginInit();
                        bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = ms;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                    }
                    OnNextClick();
                }
                catch (InvalidOperationException ex)
                {
                    ManualCheckInPanel.Visibility = Visibility.Visible;
                    LoadingOverlay.Visibility = Visibility.Collapsed;
                    ShowErrorMessage(ex.Message);
                }
                catch (Exception ex)
                {
                    ManualCheckInPanel.Visibility = Visibility.Visible;
                    LoadingOverlay.Visibility = Visibility.Collapsed;
                    ShowErrorMessage($"Unexpected error during scanning: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                ManualCheckInPanel.Visibility = Visibility.Visible;
                ShowErrorMessage($"Unexpected error: {ex.Message}");
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
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Close();
                    mainWindow.RestartApplication();
                }
            }
            catch (Exception ex)
            {
                ManualCheckInPanel.Visibility = Visibility.Visible;
                ShowErrorMessage($"Error restarting application: {ex.Message}");
            }
        }


        /// <summary>
        /// method for converting Bitmaps to base64 string.
        /// </summary>
        /// <param name="bitmapcapturedImage">The bitmapcaptured image.</param>
        /// <returns></returns>
        private string BitmapToBase64String(Bitmap bitmapcapturedImage)
            {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmapcapturedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] imageBytes = ms.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                ManualCheckInPanel.Visibility = Visibility.Visible;
                ShowErrorMessage($"Error converting bitmap to base64 string: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Shows the loader.
        /// </summary>
        private void ShowLoadingOverlay()
        {
            try
            {
                LoadingOverlay.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ManualCheckInPanel.Visibility = Visibility.Visible;
                ShowErrorMessage($"Error showing loading overlay: {ex.Message}");
            }
        }

        /// <summary>
        /// Hides the loader.
        /// </summary>
        private void HideLoadingOverlay()
        {
            try
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ManualCheckInPanel.Visibility = Visibility.Visible;
                ShowErrorMessage($"Error hiding loading overlay: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows the error message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Extracts the text from scanned document.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <returns>All text from scanned document</returns>
        public string ExtractTextFromImage(byte[] imageData)
        {
            try
            {
                string tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "eng.traineddata");

                if (!File.Exists(tessDataPath))
                {
                    throw new FileNotFoundException("Tesseract language file not found.");
                }

                using (var ocrEngine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"), "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromMemory(imageData))
                    {
                        using (var page = ocrEngine.Process(img))
                        {
                            return page.GetText();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ManualCheckInPanel.Visibility = Visibility.Visible;
                LoadingOverlay.Visibility = Visibility.Collapsed;
                VerificationMessage.Text = UIMessages.DocumentVerification.DocVerificationErrorMessage(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Parses the document details.
        /// </summary>
        /// <param name="ocrText">The ocr text.</param>
        /// <returns>Passport Number or Vietnam ID number</returns>
        public string ParseDocumentDetails(string ocrText)
        {
            try
            {
                string documentNumberPattern;

                if (_documentType == UIConstants.DocumentTypePassport)
                {
                    documentNumberPattern = UIConstants.RegexForPassport;
                }
                else if (_documentType == UIConstants.DocumentTypeVietnamId)
                {
                    documentNumberPattern =UIConstants.RegexForVietnamId;
                }
                else
                {
                    return string.Empty;
                }

                var documentNumberMatch = Regex.Match(ocrText, documentNumberPattern);
                string number = string.Empty;

                if (documentNumberMatch.Success)
                {
                    number = documentNumberMatch.Value.Replace("\n", string.Empty).Trim();
                }

                return number;
            }
            catch (Exception ex)
            {
                ManualCheckInPanel.Visibility = Visibility.Visible;
                ShowErrorMessage($"Error parsing document details: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
