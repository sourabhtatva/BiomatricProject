using BiometricAuthenticationAPI.Helpers.Enums;

namespace BiometricAuthenticationAPI.Data.Models.Response
{
    public class DocumentValidateResponse
    {
        public bool IsValid { get; set; }
        public RejectReason RejectReason { get; set; }
    }
}
