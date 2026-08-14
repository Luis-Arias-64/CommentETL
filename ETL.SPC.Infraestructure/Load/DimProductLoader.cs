using ETL.SPC.Application.Load.Interfaces;
using ETL.SPC.Domain.Base;
using NpgsqlTypes;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Infraestructure.Load
{
    public class DimProductLoader : ILoader<ProductClean>
    {
        private readonly ILogger<DimProductLoader> _logger;
        private readonly IDwhConnectionFactory _factory;

        public DimProductLoader(IDwhConnectionFactory factory, ILogger<DimProductLoader> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task LoadAsync(IEnumerable<ProductClean> items)
        {
            try
            {
                await using var connection = await DwhConnectionHelper.OpenAsync(_factory);
                await DwhConnectionHelper.TruncateAsync(connection, "dim.dimproduct");

                var distinctProducts = items
                    .GroupBy(p => p.ProductId)
                    .Select(g => g.First())
                    .OrderBy(p => p.ProductId)
                    .ToList();

                await using var writer = await connection.BeginBinaryImportAsync(
                    "COPY dim.dimproduct (productkey, productid, productname, categoryname) FROM STDIN (FORMAT BINARY)");

                var surrogateKey = 1;
                foreach (var product in distinctProducts)
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(surrogateKey, NpgsqlDbType.Integer);
                    await writer.WriteAsync(product.ProductId, NpgsqlDbType.Integer);
                    await writer.WriteAsync(product.ProductName, NpgsqlDbType.Varchar);
                    await writer.WriteAsync(product.CategoryName, NpgsqlDbType.Varchar);
                    surrogateKey++;
                }

                await writer.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                Console.WriteLine($"Correo al administrador: {ex.Message}");
            }
        }
    }
}
