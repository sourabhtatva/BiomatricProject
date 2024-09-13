namespace CheckInKiosk.Utils.Constants
{
    public static class UIMessages
    {
        public static class DocumentVerification
        {
            public static readonly Func<string, string> DocVerificationInProgressMessage = (entity) => $"Verifying given details for {entity}...";
            public const string DocVerificationSuccessMessage = "Document verified successfully!";
            public static readonly Func<string, string> DocVerificationErrorMessage = (message) => $"An error occurred: {message}";
        }

        public static class FaceVerification
        {
            public const string CameraNotFoundMessage = "No camera found!";
            public const string IdentityVerificationInProgressMessage = "We are verifying your identity. Please wait...";
            public static readonly Func<string, string> CameraStartErrorMessage = (message) => $"Error starting camera: {message}";
            public const string VerificationCancelMessage = "The verification process was canceled.";
            public static readonly Func<string, string> VerificationErrorMessage = (message) => $"Error during verification: {message}";
            public static readonly Func<string, string> VerificationApiErrorMessage = (message) => $"Verification API error: {message}";

            // Added new error messages

            public const string CameraStopErrorMessage = "Error stopping the camera.";
            public static readonly Func<string, string> NewFrameProcessingErrorMessage = (message) => $"Error processing video frame: {message}";
            public static readonly Func<string, string> CaptureImageErrorMessage = (message) => $"Error capturing image: {message}";
            public static readonly Func<string, string> BitmapConversionErrorMessage = (message) => $"Error converting bitmap: {message}";
            public const string FaceCascadeErrorMessage = "Error loading face detection cascade.";
            public static readonly Func<string, string> CaptureVerifyErrorMessage = (message) => $"Error during capture and verification process: {message}";
        }

    }
}
