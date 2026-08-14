using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;
using System.Text.Json;

namespace ETL.SPC.Application.Extract.Api
{
    public class ApiCommentExtractor : IExtractor<CommentRaw>
    {
        private readonly IApiClient _client;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiCommentExtractor(IApiClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<CommentRaw>> ExtractAsync()
        {
            try
            {
                // /api/all trae los 3 tipos de comentario (social, surveys, web-reviews)
                // en una sola llamada, tal como recomienda la documentación de la API
                // para minimizar round-trips.
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

                return socialComments.Concat(surveys).Concat(webReviews).ToList();
            }
            catch (HttpRequestException ex)
            {
                // No detenemos toda la extracción si la API externa falla;
                // se registra y se continúa con las demás fuentes.
                Console.WriteLine($"Error consultando la API de comentarios: {ex.Message}");
                return new List<CommentRaw>();
            }
        }

        // DTOs internos que mapean el contrato JSON documentado en
        // API_CSV_INGEST_CONSUMO.md (campos en camelCase). Solo se declaran los 3
        // datasets de comentarios que nos interesan; "products"/"clients"/"sources"
        // del payload de /api/all se ignoran (System.Text.Json descarta propiedades
        // no mapeadas por defecto) porque esos maestros ya se extraen de los CSV.

        private class AllResponse
        {
            public List<SocialCommentApiRow> SocialComments { get; set; } = new();
            public List<SurveyApiRow> Surveys { get; set; } = new();
            public List<WebReviewApiRow> WebReviews { get; set; } = new();
        }

        private class SocialCommentApiRow
        {
            public string IdComment { get; set; } = string.Empty;
            public string? IdCliente { get; set; } // puede venir null (~44% de las filas, según la doc)
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
