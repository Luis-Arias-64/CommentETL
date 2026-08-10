using CommentETL.Application.Extract.Interfaces;
using CommentETL.Domain.Base;

namespace CommentETL.Application.Extract.Csv
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
            var rows = CsvFileReader.Read<ClientCsvRow>(_path);

            var result = rows.Select(r => new ClientRaw
            {
                ClientId = r.IdCliente,
                Name = r.Nombre,
                Email = r.Email
            });

            return Task.FromResult(result);
        }
    }
}
