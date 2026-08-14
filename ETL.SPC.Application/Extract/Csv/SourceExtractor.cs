using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Application.Extract.Csv
{
    public class SourceExtractor : IExtractor<SourceRaw>
    {
        private readonly ILogger<SourceExtractor> _logger;
        private readonly string _path;

        public SourceExtractor(string path, ILogger<SourceExtractor> logger)
        {
            _path = path;
            _logger = logger;
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
                _logger.LogError(ex, "Enviando correo al administrador: Error extracting sources from CSV file");
                return Task.FromResult(Enumerable.Empty<SourceRaw>());
            }
        }
    }
}
