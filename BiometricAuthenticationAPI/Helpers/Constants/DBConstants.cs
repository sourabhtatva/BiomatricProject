namespace BiometricAuthenticationAPI.Helpers.Constants
{
    public static class DBConstants
    {

        public static class RecognitionLog
        {
            #region StoreProcedure
            public static readonly string RECOGNITION_LOG_INSERT = "usp_RecognitionLogInsert";
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
