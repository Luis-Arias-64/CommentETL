using CommentETL.Application.Extract.Interfaces;
using CommentETL.Domain.Base;

namespace CommentETL.Application.Extract.Csv
{
    public class WebReviewExtractor : IExtractor<CommentRaw>
    {
        private readonly string _path;

        public WebReviewExtractor(string path)
        {
            _path = path;
        }

        public Task<IEnumerable<CommentRaw>> ExtractAsync()
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
    }
}
