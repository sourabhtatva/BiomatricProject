namespace BiometricAuthenticationAPI.Helpers.Constants
{
    public class SystemConstants
    {
        public static class General
        {
            public static readonly string SUCCESS = "Success";
            public static readonly string FAILURE = "Failure";
            public static readonly string USER_ID = "UserId";
            public static readonly string ERROR_REJECT_REASON = "GeneralError";
            public static readonly string[] Reasons = { "DocumentNotFound", "PassengerIsBlackListed"};
        }
        public static class Cryptography
        {
            public static readonly byte[] ENCRYPTION_KEY = Convert.FromBase64String("dA8L8+nF8D6e0a7H0a5lY3cFg+33r0H1LK4RmO5bI3I=");
            public static readonly byte[] IV = [0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F];

        }

        public static class Configuration
        {
            public static readonly string KEY = "AzureFaceApi:Key";
            public static readonly string ENDPOINT = "AzureFaceApi:Endpoint";
            public static readonly string HEADER_VALUE = "application/octet-stream";
            public static readonly string OCP_APIM_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key";
            public static readonly string CONTENT_TYPE = "application/json";
            public static readonly float AWS_THRESHOLD_VALUE = 90F;
            public static readonly string LOG_FILE_PATH = "Logs/logfile.log";
        }

        public static class UserIdentificationData
        {
            public static readonly string ENTITY = "User ID Data";
            public static readonly string CONTROLLER_ENTITY = "User Identification Data";
        }

        public static class UserIdentificationType
        {
            public static readonly string ENTITY = "User ID Type Data";
        }

        public static class RecognitionLog
        {
            public static readonly string ENTITY = "Recognition Log Data";
        }

        public static class FaceMatching
        {
            public static readonly string CONTROLLER_ENTITY = "Face Matching Process";
        }
    }
}