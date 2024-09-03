using static System.Net.Mime.MediaTypeNames;

namespace BiometricAuthenticationAPI.Helpers.Constants
{
    public static class Messages
    {
        public static class Common
        {
            public static readonly string InvalidIdMessage = "Invalid ID provided for updating data.";
            
        }

            public static class UserIdentificationData
        {
            public static class General
            {
                public static readonly Func<string, string> NotFoundMessage = (entityName) => $"{entityName} not found.";
                public static readonly Func<string, string> ConflictMessage = (entityName) => $"{entityName} with same name already present.";
                public static readonly Func<string, string> AddMessage = (entityName) => $"{entityName} added successfully.";
                public static readonly Func<string, string> ValidateMessage = (entityName) => $"{entityName} validated successfully.";
                public static readonly Func<string, string> UpdateMessage = (entityName) => $"{entityName} updated successfully.";
                public static readonly Func<string, string> DeleteMessage = (entityName) => $"{entityName} deleted successfully.";
                public static readonly Func<string, string> AddError = (entityName) => $"Error occurred while adding {entityName}.";
                public static readonly Func<string, string> UpdateError = (entityName) => $" {entityName}.";
                public static readonly Func<string, string> DeleteError = (entityName) => $"Error occurred while deleting {entityName}.";
                public static readonly Func<string, string> ValidateErrorMessage = (entityName) => $"Error occurred while validating {entityName}.";
            }
        }

        public static class UserIdentificationType
        {
            public static class General
            {
                public static readonly Func<string, string> NotFoundMessage = (entityName) => $"{entityName} not found.";
                public static readonly Func<string, string> ConflictMessage = (entityName) => $"{entityName} with same name already present.";
                public static readonly Func<string, string> AddMessage = (entityName) => $"{entityName} added successfully.";
                public static readonly Func<string, string> UpdateMessage = (entityName) => $"{entityName} updated successfully.";
                public static readonly Func<string, string> DeleteMessage = (entityName) => $"{entityName} deleted successfully.";
                public static readonly Func<string, string> AddError = (entityName) => $"Error occurred while adding {entityName}.";
                public static readonly Func<string, string> UpdateError = (entityName) => $"Error occurred while updating {entityName}.";
                public static readonly Func<string, string> DeleteError = (entityName) => $"Error occurred while deleting {entityName}.";
            }
        }

        //public static class FaceData
        //{
        //    public static class General
        //    {
        //        public static readonly Func<string, string> NotFoundMessage = (entityName) => $"{entityName} not found.";
        //        public static readonly Func<string, string> ConflictMessage = (entityName) => $"{entityName} with same name already present.";
        //        public static readonly Func<string, string> AddMessage = (entityName) => $"{entityName} added successfully.";
        //        public static readonly Func<string, string> UpdateMessage = (entityName) => $"{entityName} updated successfully.";
        //        public static readonly Func<string, string> DeleteMessage = (entityName) => $"{entityName} deleted successfully.";
        //        public static readonly Func<string, string> AddError = (entityName) => $"Error occurred while adding {entityName}.";
        //        public static readonly Func<string, string> UpdateError = (entityName) => $"Error occurred while updating {entityName}.";
        //        public static readonly Func<string, string> DeleteError = (entityName) => $"Error occurred while deleting {entityName}.";
        //    }
        //}

        public static class RecognitionLog
        {
            public static class General
            {
                public static readonly Func<string, string> NotFoundMessage = (entityName) => $"{entityName} not found.";
                public static readonly Func<string, string> ConflictMessage = (entityName) => $"{entityName} with same name already present.";
                public static readonly Func<string, string> AddMessage = (entityName) => $"{entityName} added successfully.";
                public static readonly Func<string, string> UpdateMessage = (entityName) => $"{entityName} updated successfully.";
                public static readonly Func<string, string> DeleteMessage = (entityName) => $"{entityName} deleted successfully.";
                public static readonly Func<string, string> AddError = (entityName) => $"Error occurred while adding {entityName}.";
                public static readonly Func<string, string> UpdateError = (entityName) => $"Error occurred while updating {entityName}.";
                public static readonly Func<string, string> DeleteError = (entityName) => $"Error occurred while deleting {entityName}.";
            }
        }

        public static class FaceMatching
        {
            public static class General
            {
                public static readonly string DetectFailureMessage = "Unable to detect faces in one or both images.";
                public static readonly string VerifyFailureMessage = "Unable to verify faces in images.";
                public static readonly Func<string, string> FaceMatchingMessage = (entityName) => $"{entityName} completed successfully.";
            }
        }
    }
}
