namespace BiometricAuthenticationAPI.Data.Models.Response
{
    public class APIErrorResponse
    {
        public bool ApiFailedStatus { get; set; } = false;
        public string? ErrorMessage { get; set; }
    }
}
