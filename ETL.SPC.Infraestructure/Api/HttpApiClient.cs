using ETL.SPC.Application.Extract.Api;

namespace ETL.SPC.Infraestructure.Api
{
    // Implementación concreta de IApiClient usando HttpClient (registrado como
    // typed client vía AddHttpClient en el Worker, así ya trae BaseAddress configurada).
    public class HttpApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        public HttpApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAsync(string endpoint)
        {
            return await _httpClient.GetStringAsync(endpoint);
        }
    }
}
