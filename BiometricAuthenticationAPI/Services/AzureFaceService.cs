using BiometricAuthenticationAPI.Data.Models.Response;
using BiometricAuthenticationAPI.Services.Interfaces;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using BiometricAuthenticationAPI.Helpers.Constants;

namespace BiometricAuthenticationAPI.Services
{
    public class AzureFaceService(HttpClient httpClient, IConfiguration configuration) : IAzureFaceService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _subscriptionKey = configuration[SystemConstants.Configuration.KEY];
        private readonly string _endpoint = configuration[SystemConstants.Configuration.ENDPOINT];
        public async Task<string?> DetectFaceAsync(byte[] image)
        {
            try
            {
                using var content = new ByteArrayContent(image);
                content.Headers.ContentType = new MediaTypeHeaderValue(SystemConstants.Configuration.HEADER_VALUE);

                var requestUri = $"{_endpoint}{APIEndpointURL.FaceAPI.DETECTION_API}";
                _httpClient.DefaultRequestHeaders.Add(SystemConstants.Configuration.OCP_APIM_SUBSCRIPTION_KEY, _subscriptionKey);

                var response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var faceResponse = JsonSerializer.Deserialize<FaceDetectResponse[]>(jsonResponse);

                return faceResponse?.Length > 0 ? faceResponse[0].FaceId : null;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<FaceVerifyResponse?> VerifyFacesAsync(string faceId1, string faceId2)
        {
            try
            {
                var verifyRequest = new
                {
                    faceId1,
                    faceId2
                };

                var content = new StringContent(JsonSerializer.Serialize(verifyRequest), Encoding.UTF8, SystemConstants.Configuration.CONTENT_TYPE);

                var requestUri = $"{_endpoint}{APIEndpointURL.FaceAPI.VERIFY_API}";
                _httpClient.DefaultRequestHeaders.Add(SystemConstants.Configuration.OCP_APIM_SUBSCRIPTION_KEY, _subscriptionKey);

                var response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<FaceVerifyResponse>(jsonResponse);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
           
        }
    }
}
