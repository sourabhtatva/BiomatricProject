using BiometricAuthenticationAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BiometricAuthenticationAPI.Services
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SetData(string key, string data)
        {
            _memoryCache.Set(key, data);
        }

        public string? GetData(string key)
        {
            string? data;
            _memoryCache.TryGetValue(key, out data);
            return data;
        }
    }
}
