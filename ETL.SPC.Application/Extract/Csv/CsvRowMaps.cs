using CsvHelper.Configuration.Attributes;

namespace ETL.SPC.Application.Extract.Csv
{
    // Estas clases solo existen para que CsvHelper mapee las columnas de cada archivo
    // por nombre de encabezado (evita el parseo manual con Split(',') que se rompe
    // cuando un comentario trae comas dentro de comillas).

    public class ClientCsvRow
    {
        [Name("IdCliente")] public string IdCliente { get; set; } = string.Empty;
        [Name("Nombre")] public string Nombre { get; set; } = string.Empty;
        [Name("Email")] public string Email { get; set; } = string.Empty;
    }

    public class ProductCsvRow
    {
        [Name("IdProducto")] public string IdProducto { get; set; } = string.Empty;
        [Name("Nombre")] public string Nombre { get; set; } = string.Empty;
        [Name("Categoría", "Categoria")] public string Categoria { get; set; } = string.Empty;
    }

    public class SourceCsvRow
    {
        [Name("IdFuente")] public string IdFuente { get; set; } = string.Empty;
        [Name("TipoFuente")] public string TipoFuente { get; set; } = string.Empty;
        [Name("FechaCarga")] public string FechaCarga { get; set; } = string.Empty;
    }

    public class SocialCommentCsvRow
    {
        [Name("IdComment")] public string IdComment { get; set; } = string.Empty;
        [Name("IdCliente")] public string IdCliente { get; set; } = string.Empty;
        [Name("IdProducto")] public string IdProducto { get; set; } = string.Empty;
        [Name("Fuente")] public string Fuente { get; set; } = string.Empty;
        [Name("Fecha")] public string Fecha { get; set; } = string.Empty;
        [Name("Comentario")] public string Comentario { get; set; } = string.Empty;
    }

    public class SurveyCsvRow
    {
        [Name("IdOpinion")] public string IdOpinion { get; set; } = string.Empty;
        [Name("IdCliente")] public string IdCliente { get; set; } = string.Empty;
        [Name("IdProducto")] public string IdProducto { get; set; } = string.Empty;
        [Name("Fecha")] public string Fecha { get; set; } = string.Empty;
        [Name("Comentario")] public string Comentario { get; set; } = string.Empty;
        [Name("Clasificación", "Clasificacion")] public string Clasificacion { get; set; } = string.Empty;
        [Name("PuntajeSatisfacción", "PuntajeSatisfaccion")] public string PuntajeSatisfaccion { get; set; } = string.Empty;
        [Name("Fuente")] public string Fuente { get; set; } = string.Empty;
    }

    public class WebReviewCsvRow
    {
        [Name("IdReview")] public string IdReview { get; set; } = string.Empty;
        [Name("IdCliente")] public string IdCliente { get; set; } = string.Empty;
        [Name("IdProducto")] public string IdProducto { get; set; } = string.Empty;
        [Name("Fecha")] public string Fecha { get; set; } = string.Empty;
        [Name("Comentario")] public string Comentario { get; set; } = string.Empty;
        [Name("Rating")] public string Rating { get; set; } = string.Empty;
    }
}
