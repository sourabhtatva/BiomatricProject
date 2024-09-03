namespace BiometricAuthenticationAPI.Data.Models.Common
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public object? Data { get; set; }

        public int Code { get; set; }
    }
}
