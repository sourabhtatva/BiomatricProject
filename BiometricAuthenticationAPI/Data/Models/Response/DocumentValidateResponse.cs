namespace BiometricAuthenticationAPI.Data.Models.Response
{
    public class DocumentValidateResponse : APIErrorResponse
    {
        public int? UserId { get; set; }
        public bool IsValid { get; set; }
        public string? RejectReason { get; set; }
    }
}
