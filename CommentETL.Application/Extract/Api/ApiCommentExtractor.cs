using CommentETL.Application.Extract.Interfaces;
using CommentETL.Domain.Base;
using System.Text.Json;

namespace CommentETL.Application.Extract.Api
{
    public class ApiCommentExtractor : IExtractor<CommentRaw>
    {
        private readonly IApiClient _client;

        public ApiCommentExtractor(IApiClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<CommentRaw>> ExtractAsync()
        {
            try
            {
                var json = await _client.GetAsync("/comments");

                var data = JsonSerializer.Deserialize<List<CommentRaw>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return data ?? new List<CommentRaw>();
            }
            catch (HttpRequestException ex)
            {
                // No detenemos toda la extracción si la API externa falla;
                // se registra y se continúa con las demás fuentes.
                Console.WriteLine($"Error consultando la API de comentarios: {ex.Message}");
                return new List<CommentRaw>();
            }
        }
    }
}
