using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Application.Extract.Api
{
    public class ApiCommentExtractor : IExtractorExternal<CommentRaw>
    {
        private readonly IApiClient _client;
        private readonly ILogger<ApiCommentExtractor> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiCommentExtractor(IApiClient client, ILogger<ApiCommentExtractor> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<IQueryable<CommentRaw>> ExtractAsync()
        {
            try
            {
                var json = await _client.GetAsync("/api/all");

                var data = JsonSerializer.Deserialize<AllResponse>(json, JsonOptions) ?? new AllResponse();

                var socialComments = data.SocialComments.Select(r => new CommentRaw
                {
                    SourceType = "Social",
                    ClientId = r.IdCliente,
                    ProductId = r.IdProducto,
                    SourceName = r.Fuente,
                    Date = r.Fecha,
                    Comment = r.Comentario
                });

                var surveys = data.Surveys.Select(r => new CommentRaw
                {
                    SourceType = "Survey",
                    ClientId = r.IdCliente,
                    ProductId = r.IdProducto,
                    Date = r.Fecha,
                    Comment = r.Comentario,
                    Sentiment = r.Clasificacion,
                    Rating = r.PuntajeSatisfaccion,
                    SourceName = r.Fuente
                });

                var webReviews = data.WebReviews.Select(r => new CommentRaw
                {
                    SourceType = "Web",
                    ClientId = r.IdCliente,
                    ProductId = r.IdProducto,
                    Date = r.Fecha,
                    Comment = r.Comentario,
                    Rating = r.Rating
                });

                return socialComments.Concat(surveys).Concat(webReviews).AsQueryable();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Enviando correo al administrador: Error consultando la API de comentarios");
                return new List<CommentRaw>().AsQueryable();
            }
        }
        private class AllResponse
        {
            public List<SocialCommentApiRow> SocialComments { get; set; } = new();
            public List<SurveyApiRow> Surveys { get; set; } = new();
            public List<WebReviewApiRow> WebReviews { get; set; } = new();
        }

        private class SocialCommentApiRow
        {
            public string IdComment { get; set; } = string.Empty;
            public string? IdCliente { get; set; } // puede venir null
            public string IdProducto { get; set; } = string.Empty;
            public string Fuente { get; set; } = string.Empty;
            public DateTime Fecha { get; set; }
            public string Comentario { get; set; } = string.Empty;
        }

        private class SurveyApiRow
        {
            public int IdOpinion { get; set; }
            public string? IdCliente { get; set; }
            public string IdProducto { get; set; } = string.Empty;
            public DateTime Fecha { get; set; }
            public string Comentario { get; set; } = string.Empty;
            public string Clasificacion { get; set; } = string.Empty;
            public int PuntajeSatisfaccion { get; set; }
            public string Fuente { get; set; } = string.Empty;
        }

        private class WebReviewApiRow
        {
            public string IdReview { get; set; } = string.Empty;
            public string? IdCliente { get; set; }
            public string IdProducto { get; set; } = string.Empty;
            public DateTime Fecha { get; set; }
            public string Comentario { get; set; } = string.Empty;
            public int Rating { get; set; }
        }
    }
}
