using System.IO;
using System.Text.Json;

namespace CheckInKiosk.Utils.Constants
{
    public class LogConfig
    {
        public LogMessage? LogMessages { get; set; }
        public APIEndpoint? APIEndpoints { get; set; }
        public UIConstant? UIConstants { get; set; }

        public static LogConfig Load(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<LogConfig>(json);
        }

        public class LogMessage
        {
            public string ScanSuccess { get; set; }
            public string ScanFailure { get; set; }
            public string ErrorSelectingDocumentType { get; set; }
            public string ErrorProcessingDocumentType { get; set; }
            public string UnexpectedError { get; set; }
            public string RequestTimeout { get; set; }
            public string NetworkError { get; set; }
            public string ParsingError { get; set; }
            public string DocumentVerificationInProgress { get; set; }
            public string DocumentVerifiedSuccess { get; set; }
            public string NoScannerFound { get; set; }
            public string ErrorDuringScanning { get; set; }
            public string ErrorRestartingApplication { get; set; }
            public string FaceCascadeErrorMessage { get; set; }
            public string CameraNotFoundMessage { get; set; }
            public string CameraStartErrorMessage { get; set; }
            public string CameraStopErrorMessage { get; set; }
            public string NewFrameProcessingErrorMessage { get; set; }
            public string BitmapConversionErrorMessage { get; set; }
            public string IdentityVerificationInProgressMessage { get; set; }
            public string CaptureVerifyErrorMessage { get; set; }
            public string VerificationCancelMessage { get; set; }
            public string VerificationErrorMessage { get; set; }
            public string ErrorDuringShowingLoadingOverlay { get; set; }
            public string ErrorDuringHidingLoadingOverlay { get; set; }
            public string VerificationApiErrorMessage { get; set; }
        }

        public class APIEndpoint
        {
            public string ValidateDocAPI { get; set; }
            public string FaceMatchingAPI { get; set; }
        }

        public class UIConstant
        {
            public string ContentType { get; set; }
            public string EncryptionKey { get; set; }
            public string HaarcascadeFrontalfacePath { get; set; }
            public string Filter { get; set; }
            public string DocumentTypePassport { get; set; }
            public string DocumentTypeVietnamID { get; set; }
        }
    }
}
