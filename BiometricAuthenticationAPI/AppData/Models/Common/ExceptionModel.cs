using System.Text.Json;
namespace BiometricAuthenticationAPI.Data.Models.Common
{
    /// <summary>
    /// Error details to setup formated error messages.
    /// </summary>
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
        public bool IsSuccess { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    /// <summary>
    /// Custom Validation error model.
    /// </summary>
    public class ValidationDetails
    {
        public string? InputName { get; set; }
        public string? ValidationMessage { get; set; }
    }
}
