using Amazon.Rekognition.Model;
using BiometricAuthenticationAPI.Data.Models.Response;
using BiometricAuthenticationAPI.Services.Interfaces;

namespace BiometricAuthenticationAPI.Services
{
    public class AwsFaceService(HttpClient httpClient) : IAwsFaceService
    {
        private readonly HttpClient _httpClient = httpClient;

        public Task<FaceVerifyResponse?> VerifyFacesAsync(CompareFacesRequest compareFacesRequest)
        {
            throw new NotImplementedException();
        }
    }
}
