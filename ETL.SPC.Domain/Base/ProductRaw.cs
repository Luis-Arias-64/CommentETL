namespace ETL.SPC.Domain.Base
{
    // Representa una fila cruda de products.csv, sin validar ni transformar.
    public class ProductRaw
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
