using System.Net.Http;

namespace CheckInKiosk.Utils.Services
{
        public class HttpClientFactory
        {
            private static readonly Lazy<HttpClient> lazyHttpClient = new Lazy<HttpClient>(() =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:5062/")
                };
                return client;
            });

            public static HttpClient Instance => lazyHttpClient.Value;
        }
}
