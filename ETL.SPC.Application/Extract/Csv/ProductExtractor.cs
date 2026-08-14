using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Application.Extract.Csv
{
    public class ProductExtractor : IExtractor<ProductRaw>
    {
        private readonly ILogger<ProductExtractor> _logger;
        private readonly string _path;

        public ProductExtractor(string path, ILogger<ProductExtractor> logger)
        {
            _path = path;
            _logger = logger;
        }

        public Task<IEnumerable<ProductRaw>> ExtractAsync()
        {
            try
            {
                var rows = CsvFileReader.Read<ProductCsvRow>(_path);

                var result = rows.Select(r => new ProductRaw
                {
                    ProductId = r.IdProducto,
                    Name = r.Nombre,
                    Category = r.Categoria
                });

                return Task.FromResult(result);    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enviando correo al administrador: Error extracting products from CSV file");
                return Task.FromResult(Enumerable.Empty<ProductRaw>());
            }
        }
    }
}
