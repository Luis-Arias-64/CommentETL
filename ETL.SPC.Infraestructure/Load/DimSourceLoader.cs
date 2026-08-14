using ETL.SPC.Application.Load.Interfaces;
using ETL.SPC.Domain.Base;
using NpgsqlTypes;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Infraestructure.Load
{
    public class DimSourceLoader : ILoader<SourceClean>
    {
        private readonly ILogger<DimSourceLoader> _logger;
        private readonly IDwhConnectionFactory _factory;

        public DimSourceLoader(IDwhConnectionFactory factory, ILogger<DimSourceLoader> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task LoadAsync(IEnumerable<SourceClean> items)
        {
            try{
                await using var connection = await DwhConnectionHelper.OpenAsync(_factory);
                await DwhConnectionHelper.TruncateAsync(connection, "dim.dimsource");

                var distinctSources = items
                    .GroupBy(s => s.SourceId)
                    .Select(g => g.First())
                    .OrderBy(s => s.SourceId)
                    .ToList();

                await using var writer = await connection.BeginBinaryImportAsync(
                    "COPY dim.dimsource (sourcekey, sourceid, sourcename, loaddate) FROM STDIN (FORMAT BINARY)");

                var surrogateKey = 1;
                foreach (var source in distinctSources)
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(surrogateKey, NpgsqlDbType.Integer);
                    await writer.WriteAsync(source.SourceId, NpgsqlDbType.Integer);
                    await writer.WriteAsync(source.SourceType, NpgsqlDbType.Varchar);
                    await writer.WriteAsync(source.LoadDate.Date, NpgsqlDbType.Date);
                    surrogateKey++;
                }

                await writer.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sources");
                Console.WriteLine($"Correo al administrador: {ex.Message}");
            }
        }
    }
}
