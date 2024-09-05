using BiometricAuthenticationAPI.Data.Models.Response;
using BiometricAuthenticationAPI.Helpers.Constants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
