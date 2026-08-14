namespace ETL.SPC.Domain.Base
{
    // Salida de Transform para comentarios/opiniones. Usa claves naturales
    public class CommentClean
    {
        public string SourceType { get; set; } = string.Empty; // Survey | Web | Social | DB | Api
        public int? ClientId { get; set; }
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; } = string.Empty;
        public decimal? Rating { get; set; }
        public string Sentiment { get; set; } = string.Empty;   // Positiva | Neutra | Negativa
        public string? SourceName { get; set; }
    }
}
