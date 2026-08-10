using CommentETL.Application.Extract.Interfaces;
using CommentETL.Domain.Base;

namespace CommentETL.Application.Extract.Csv
{
    public class SurveyExtractor : IExtractor<CommentRaw>
    {
        private readonly string _path;

        public SurveyExtractor(string path)
        {
            _path = path;
        }

        public Task<IEnumerable<CommentRaw>> ExtractAsync()
        {
            var rows = CsvFileReader.Read<SurveyCsvRow>(_path);

            var result = rows.Select(r => new CommentRaw
            {
                SourceType = "Survey",
                ClientId = string.IsNullOrWhiteSpace(r.IdCliente) ? null : r.IdCliente,
                ProductId = string.IsNullOrWhiteSpace(r.IdProducto) ? null : r.IdProducto,
                Date = DateTime.TryParse(r.Fecha, out var fecha) ? fecha : DateTime.MinValue,
                Comment = r.Comentario,
                Sentiment = r.Clasificacion,
                Rating = int.TryParse(r.PuntajeSatisfaccion, out var rating) ? rating : null,
                SourceName = r.Fuente
            });

            return Task.FromResult(result);
        }
    }
}
