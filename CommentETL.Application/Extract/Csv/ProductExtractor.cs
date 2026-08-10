using CommentETL.Application.Extract.Interfaces;
using CommentETL.Domain.Base;

namespace CommentETL.Application.Extract.Csv
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
            var rows = CsvFileReader.Read<ProductCsvRow>(_path);

            var result = rows.Select(r => new ProductRaw
            {
                ProductId = r.IdProducto,
                Name = r.Nombre,
                Category = r.Categoria
            });

            return Task.FromResult(result);
        }
    }
}
