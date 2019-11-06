using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication.Proxies
{
    public class CityHttpClient : ICityClient
    {
        private readonly HttpClient _httpClient;

        public CityHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> Get()
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "city");

            HttpResponseMessage httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new ApplicationException($"Response status code is {httpResponseMessage.StatusCode}");   
            }

            string content = await httpResponseMessage.Content.ReadAsStringAsync();

            return content;
        }
    }
}