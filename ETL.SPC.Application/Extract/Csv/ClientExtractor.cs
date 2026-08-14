using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;

namespace ETL.SPC.Application.Extract.Csv
{
    public class ClientExtractor : IExtractor<ClientRaw>
    {
        private readonly string _path;

        public ClientExtractor(string path)
        {
            _path = path;
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
                Console.WriteLine($"Enviando correo al administrador: Error extracting clients from CSV file: {ex.Message}");
                return Task.FromResult(Enumerable.Empty<ClientRaw>());
            }
        }
    }
}
