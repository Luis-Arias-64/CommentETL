namespace CommentETL.Domain.Base
{
    // Representa una fila cruda de fuente_datos.csv, sin validar ni transformar.
    public class SourceRaw
    {
        public string SourceId { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string LoadDate { get; set; } = string.Empty;
    }
}
