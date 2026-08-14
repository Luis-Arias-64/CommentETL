using ETL.SPC.Application.Load.Interfaces;
using ETL.SPC.Domain.Base;
using NpgsqlTypes;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Infraestructure.Load
{
    public class DimClientLoader : ILoader<ClientClean>
    {
        private readonly ILogger<DimClientLoader> _logger;
        private readonly IDwhConnectionFactory _factory;

        public DimClientLoader(IDwhConnectionFactory factory, ILogger<DimClientLoader> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task LoadAsync(IEnumerable<ClientClean> items)
        {
                try{
                await using var connection = await DwhConnectionHelper.OpenAsync(_factory);
                await DwhConnectionHelper.TruncateAsync(connection, "dim.dimclient");

                var distinctClients = items
                    .Select(c => c.ClientId)
                    .Distinct()
                    .OrderBy(id => id)
                    .ToList();

                // COPY binario: el mecanismo recomendado por Npgsql para cargas masivas
                // a un DWH, muy por encima de hacer un INSERT por fila.
                await using var writer = await connection.BeginBinaryImportAsync(
                    "COPY dim.dimclient (clientkey, clientid) FROM STDIN (FORMAT BINARY)");

                var surrogateKey = 1;
                foreach (var clientId in distinctClients)
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(surrogateKey, NpgsqlDbType.Integer);
                    await writer.WriteAsync(clientId, NpgsqlDbType.Integer);
                    surrogateKey++;
                }

                await writer.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading clients");
                Console.WriteLine($"Correo al administrador: {ex.Message}");
            }
        }
    }
}
