using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;

namespace ETL.SPC.Application.Extract.Csv
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
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Enviando correo al administrador: Error extracting survey comments from CSV file: {ex.Message}");
                return Task.FromResult(Enumerable.Empty<CommentRaw>());
            }
        }
    }
}
