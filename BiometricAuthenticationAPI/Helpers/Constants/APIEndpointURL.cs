namespace BiometricAuthenticationAPI.Helpers.Constants
{
    public class APIEndpointURL
    {
        public static class FaceAPI
        {
            public static readonly string DETECTION_API = "/face/v1.0/detect?returnFaceId=true";
            public static readonly string VERIFY_API = "/face/v1.0/verify";
        }
    }
}
