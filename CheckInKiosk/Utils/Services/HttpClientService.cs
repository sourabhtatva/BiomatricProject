using System.Net.Http;

namespace CheckInKiosk.Utils.Services
{
    public class HttpClientService
    {
        public readonly HttpClient _httpClient = HttpClientFactory.Instance;
        public async Task<string> PostAsync(string endpointURL, StringContent content)
        {
            try
            {
                var response = await _httpClient.PostAsync(endpointURL, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
