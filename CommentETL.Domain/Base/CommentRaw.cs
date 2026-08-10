namespace CommentETL.Domain.Base
{
    public class CommentRaw
    {
        public string SourceType { get; set; } = string.Empty;

        public string? ClientId { get; set; }
        public string? ProductId { get; set; }

        public DateTime Date { get; set; }

        public string Comment { get; set; } = string.Empty;

        public int? Rating { get; set; }
        public string? Sentiment { get; set; }

        public string? SourceName { get; set; }
    }
}