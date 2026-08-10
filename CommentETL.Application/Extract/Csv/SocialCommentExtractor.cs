using CommentETL.Application.Extract.Interfaces;
using CommentETL.Domain.Base;

namespace CommentETL.Application.Extract.Csv
{
    public class SocialCommentExtractor : IExtractor<CommentRaw>
    {
        private readonly string _path;

        public SocialCommentExtractor(string path)
        {
            _path = path;
        }

        public Task<IEnumerable<CommentRaw>> ExtractAsync()
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
    }
}
