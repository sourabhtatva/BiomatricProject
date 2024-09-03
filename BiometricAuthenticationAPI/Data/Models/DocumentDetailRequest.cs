namespace BiometricAuthenticationAPI.Data.Models
{
    public class DocumentDetailRequest
    {
        public required string DocumentNumber { get; set; }
        public required string DocumentType { get; set; }
    }
}
