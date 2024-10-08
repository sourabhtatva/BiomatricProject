﻿namespace BiometricAuthenticationAPI.Helpers.Constants
{
    public class SystemConstants
    {
        public static class General
        {
            public static readonly string SUCCESS = "Success";
            public static readonly string FAILURE = "Failure";
        }

        public static class Configuration
        {
            public static readonly string KEY = "AzureFaceApi:Key";
            public static readonly string ENDPOINT = "AzureFaceApi:Endpoint";
            public static readonly string HEADER_VALUE = "application/octet-stream";
            public static readonly string OCP_APIM_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key";
            public static readonly string CONTENT_TYPE = "application/json";
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