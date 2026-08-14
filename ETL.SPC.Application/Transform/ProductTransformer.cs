using ETL.SPC.Application.Transform.Filters;
using ETL.SPC.Application.Transform.Interfaces;
using ETL.SPC.Application.Transform.Processing;
using ETL.SPC.Domain.Base;

namespace ETL.SPC.Application.Transform
{
    public class ProductTransformer : ITransformer<ProductRaw, ProductClean>
    {
        public IEnumerable<ProductClean> Transform(IEnumerable<ProductRaw> input)
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

        private static ProductClean? ToClean(ProductRaw raw)
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
    }
}
