namespace CommentETL.Domain.Entities.Dimensions
{
    public class DimSentiment
    {
        public int SentimentKey { get; set; }

        public string Sentiment { get; set; } = string.Empty;
    }
}