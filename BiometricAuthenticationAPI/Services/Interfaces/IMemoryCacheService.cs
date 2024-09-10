namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IMemoryCacheService
    {
        void SetData(string key, string data);
        string? GetData(string key);
    }
}
