namespace BiometricAuthenticationAPI.Data.Models.Common
{
    /// <summary>
    /// Custom Validation error model.
    /// </summary>
    public class ValidationDetails
    {
        public string? InputName { get; set; }
        public string? ValidationMessage { get; set; }
    }
}
