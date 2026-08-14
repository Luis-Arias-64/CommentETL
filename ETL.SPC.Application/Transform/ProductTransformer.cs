using ETL.SPC.Application.Transform.Filters;
using ETL.SPC.Application.Transform.Interfaces;
using ETL.SPC.Application.Transform.Processing;
using ETL.SPC.Domain.Base;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Application.Transform
{
    public class ProductTransformer : ITransformer<ProductRaw, ProductClean>
    {
        private readonly ILogger<ProductTransformer> _logger;

        public ProductTransformer(ILogger<ProductTransformer> logger)
        {
            _logger = logger;
        }
        public IEnumerable<ProductClean> Transform(IEnumerable<ProductRaw> input)
        {
            try
            {
                var withRequiredFields = RequiredFieldsFilter.Apply(input);

                var normalized = withRequiredFields
                    .Select(ToClean)
                    .Where(p => p is not null)
                    .Select(p => p!)
                    .ToList();

                return DuplicateFilter.Apply(normalized, p =>
                    $"{p.ProductName.ToLowerInvariant()}|{p.CategoryName.ToLowerInvariant()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enviando correo al administrador: Error transforming products");
                return Enumerable.Empty<ProductClean>();
            }
        }

        private static ProductClean? ToClean(ProductRaw raw)
        {
            try
            {
                var id = TextNormalizer.ParseId(raw.ProductId);
                if (id is null) return null;

                return new ProductClean
                {
                    ProductId = id.Value,
                    ProductName = TextNormalizer.CleanComment(raw.Name),
                    CategoryName = TextNormalizer.CleanComment(raw.Category)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error transforming product {raw.ProductId}: {ex.Message}");
                return null;
            }
        }
    }
}
