namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IMemoryCacheService
    {
        void SetData(string key, string data);
        void RemoveData(string key);
        string? GetData(string key);
    }
}
