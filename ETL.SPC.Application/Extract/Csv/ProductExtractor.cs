using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;

namespace ETL.SPC.Application.Extract.Csv
{
    public class ProductExtractor : IExtractor<ProductRaw>
    {
        private readonly string _path;

        public ProductExtractor(string path)
        {
            _path = path;
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
                Console.WriteLine($"Enviando correo al administrador: Error extracting products from CSV file: {ex.Message}");
                return Task.FromResult(Enumerable.Empty<ProductRaw>());
            }
        }
    }
}
