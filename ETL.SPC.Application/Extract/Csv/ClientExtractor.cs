using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;
using Microsoft.Extensions.Logging;
namespace ETL.SPC.Application.Extract.Csv
{
    public class ClientExtractor : IExtractor<ClientRaw>
    {
        private readonly string _path;
        private readonly ILogger<ClientExtractor> _logger;

        public ClientExtractor(string path, ILogger<ClientExtractor> logger)
        {
            _path = path;
            _logger = logger;
        }   
        public Task<IEnumerable<ClientRaw>> ExtractAsync()
        {
            try
            {
                var rows = CsvFileReader.Read<ClientCsvRow>(_path);
                var result = rows.Select(r => new ClientRaw
                {
                    ClientId = r.IdCliente,
                    Name = r.Nombre,
                    Email = r.Email
                });
                return Task.FromResult(result);                
            
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Enviando correo al administrador: Error extracting clients from CSV file");
                return Task.FromResult(Enumerable.Empty<ClientRaw>());
            }
        }
    }
}
