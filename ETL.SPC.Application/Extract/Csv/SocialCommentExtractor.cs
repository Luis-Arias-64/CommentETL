using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Application.Extract.Csv
{
    public class SocialCommentExtractor : IExtractor<CommentRaw>
    {
        private readonly ILogger<SocialCommentExtractor> _logger;
        private readonly string _path;

        public SocialCommentExtractor(string path, ILogger<SocialCommentExtractor> logger)
        {
            _path = path;
            _logger = logger;
        }

        public Task<IEnumerable<CommentRaw>> ExtractAsync()
        {
            try
            {
                var rows = CsvFileReader.Read<SocialCommentCsvRow>(_path);

                var result = rows.Select(r => new CommentRaw
                {
                    SourceType = "Social",
                    ClientId = string.IsNullOrWhiteSpace(r.IdCliente) ? null : r.IdCliente,
                    ProductId = string.IsNullOrWhiteSpace(r.IdProducto) ? null : r.IdProducto,
                    SourceName = r.Fuente,
                    Date = DateTime.TryParse(r.Fecha, out var fecha) ? fecha : DateTime.MinValue,
                    Comment = r.Comentario
                });

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enviando correo al administrador: Error extracting social comments from CSV file");
                return Task.FromResult(Enumerable.Empty<CommentRaw>());
            }
        }
    }
}
