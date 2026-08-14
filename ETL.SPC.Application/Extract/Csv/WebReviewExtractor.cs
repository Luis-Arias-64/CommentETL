using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Application.Extract.Csv
{
    public class WebReviewExtractor : IExtractor<CommentRaw>
    {
        private readonly ILogger<WebReviewExtractor> _logger;
        private readonly string _path;

        public WebReviewExtractor(string path, ILogger<WebReviewExtractor> logger)
        {
            _path = path;
            _logger = logger;
        }

        public Task<IEnumerable<CommentRaw>> ExtractAsync()
        {
            try
            {
                var rows = CsvFileReader.Read<WebReviewCsvRow>(_path);

                var result = rows.Select(r => new CommentRaw
                {
                    SourceType = "Web",
                    ClientId = string.IsNullOrWhiteSpace(r.IdCliente) ? null : r.IdCliente,
                    ProductId = string.IsNullOrWhiteSpace(r.IdProducto) ? null : r.IdProducto,
                    Date = DateTime.TryParse(r.Fecha, out var fecha) ? fecha : DateTime.MinValue,
                    Comment = r.Comentario,
                    Rating = int.TryParse(r.Rating, out var rating) ? rating : null
                });
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enviando correo al administrador: Error extracting web reviews from CSV file");
                return Task.FromResult(Enumerable.Empty<CommentRaw>());
            }
        }
    }
}
