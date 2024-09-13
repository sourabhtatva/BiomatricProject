namespace BiometricAuthenticationAPI.Helpers.Constants
{
    public static class DBConstants
    {
        public static class UserIdentificationData
        {
            #region StoreProcedure
            public static readonly string GET_USER_IDENTIFICATION_DATA = "usp_GetUserIdentificationData";
            public static readonly string GET_USER_IDENTIFICATION_DATA_BY_ID = "usp_GetUserIdentificationDataById";
            public static readonly string GET_USER_IDENTIFICATION_DATA_BY_DOCUMENT_NUMBER = "usp_GetUserIdentificationDataByDocumentNumber";
            public static readonly string USER_IDENTIFICATION_DATA_INSERT = "usp_UserIdentificationDataInsert";
            public static readonly string USER_IDENTIFICATION_DATA_UPDATE = "usp_UserIdentificationDataUpdate";
            public static readonly string USER_IDENTIFICATION_DATA_DELETE = "usp_UserIdentificationDataDelete";
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
            public static readonly string DOCUMENT_NUMBER = "DocumentNumber";
            public static readonly string DOCUMENT_TYPE = "DocumentType";
            #endregion
        }

        public static class UserIdentificationType
        {
            #region StoreProcedure
            public static readonly string GET_USER_IDENTIFICATION_TYPE = "usp_GetUserIdentificationType";
            public static readonly string GET_USER_IDENTIFICATION_TYPE_BY_ID = "usp_GetUserIdentificationTypeById";
            public static readonly string GET_USER_IDENTIFICATION_TYPE_BY_TYPE = "usp_GetUserIdentificationTypeByType";
            #endregion

            #region DbParameters
            public static readonly string ID = "Id";
            public static readonly string DOCUMENT_TYPE = "DocumentType";
            #endregion
        }

        public static class RecognitionLog
        {
            #region StoreProcedure
            public static readonly string GET_RECOGNITION_LOG_DATA = "usp_GetRecognitionLogData";
            public static readonly string GET_RECOGNITION_LOG_DATA_BY_ID = "usp_GetRecognitionLogDataById";
            public static readonly string RECOGNITION_LOG_INSERT = "usp_RecognitionLogInsert";
            public static readonly string RECOGNITION_LOG_UPDATE = "usp_RecognitionLogUpdate";
            public static readonly string RECOGNITION_LOG_DELETE = "usp_RecognitionLogDelete";
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

        public static class DocumentValidationResponse
        {
            #region StoreProcedure
            public static readonly string DOCUMENT_VALIDATION_SP = "usp_DocumentValidation";
            #endregion

            #region DbParameters
            public static readonly string DOCUMENT_NUMBER = "DocumentNumber";
            public static readonly string DOCUMENT_TYPE = "DocumentType";
            #endregion
        }
    }
}
