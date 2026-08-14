using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;

namespace ETL.SPC.Application.Extract.Csv
{
    public class SourceExtractor : IExtractor<SourceRaw>
    {
        private readonly string _path;

        public SourceExtractor(string path)
        {
            _path = path;
        }

        public Task<IEnumerable<SourceRaw>> ExtractAsync()
        {
            try
            {
                var rows = CsvFileReader.Read<SourceCsvRow>(_path);

                var result = rows.Select(r => new SourceRaw
                {
                    SourceId = r.IdFuente,
                    SourceType = r.TipoFuente,
                    LoadDate = r.FechaCarga
                });

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Enviando correo al administrador: Error extracting sources from CSV file: {ex.Message}");
                return Task.FromResult(Enumerable.Empty<SourceRaw>());
            }
        }
    }
}
