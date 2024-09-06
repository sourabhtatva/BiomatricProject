namespace BiometricAuthenticationAPI.Helpers.Constants
{
    public static class DBConstants
    {
        public static class UserIdentificationData
        {
            #region StoreProcedure
            public static readonly string GET_USER_IDENTIFICATION_DATA = "get_user_identification_data";
            public static readonly string GET_USER_IDENTIFICATION_DATA_BY_ID = "get_user_identification_data_by_id";
            public static readonly string GET_USER_IDENTIFICATION_DATA_BY_DOCUMENT_NUMBER = "get_user_identification_data_by_document_number";
            public static readonly string USER_IDENTIFICATION_DATA_INSERT = "user_identification_data_insert";
            public static readonly string USER_IDENTIFICATION_DATA_UPDATE = "user_identification_data_update";
            public static readonly string USER_IDENTIFICATION_DATA_DELETE = "user_identification_data_delete";
            #endregion

            #region DbParameters
            public static readonly string ID = "Id";
            public static readonly string FIRST_NAME = "FirstName";
            public static readonly string LAST_NAME = "LastName";
            public static readonly string EMAIL = "Email";
            public static readonly string PHONE_NUMBER = "PhoneNumber";
            public static readonly string USER_ID_NUMBER = "UserIdNumber";
            public static readonly string USER_ID_TYPE = "UserIdType";
            public static readonly string USER_IMAGE = "UserImage";
            public static readonly string IS_BLACKLIST_USER = "IsBlacklistUser";
            public static readonly string ADDRESS = "Address";
            public static readonly string CITY = "City";
            public static readonly string STATE = "State";
            public static readonly string ZIP_CODE = "ZipCode";
            #endregion
        }

        public static class UserIdentificationType
        {
            #region StoreProcedure
            public static readonly string GET_USER_IDENTIFICATION_TYPE = "get_user_identification_type";
            public static readonly string GET_USER_IDENTIFICATION_TYPE_BY_ID = "get_user_identification_type_by_id";
            public static readonly string GET_USER_IDENTIFICATION_TYPE_BY_TYPE = "get_user_identification_type_by_type";
            #endregion

            #region DbParameters
            public static readonly string ID = "Id";
            public static readonly string DOCUMENT_TYPE = "DocumentType";
            #endregion
        }

        public static class RecognitionLog
        {
            #region StoreProcedure
            public static readonly string GET_RECOGNITION_LOG_DATA = "get_recognition_log_data";
            public static readonly string GET_RECOGNITION_LOG_DATA_BY_ID = "get_recognition_log_data_by_id";
            public static readonly string RECOGNITION_LOG_INSERT = "recognition_log_insert";
            public static readonly string RECOGNITION_LOG_UPDATE = "recognition_log_update";
            public static readonly string RECOGNITION_LOG_DELETE = "recognition_log_delete";
            #endregion

            #region DbParameters
            public static readonly string ID = "Id";
            public static readonly string USER_ID = "UserId"; 
            public static readonly string RECOGNITION_TIME = "RecognitionTime";
            public static readonly string CONFIDENCE_LEVEL = "ConfidenceLevel";
            public static readonly string SIMILARITY_LEVEL = "SimilarityLevel";
            public static readonly string STATUS = "Status";
            #endregion
        }
    }
}
