namespace CommentETL.Domain.Entities.Dimensions
{
    public class DimSource
    {
        public int SourceKey { get; set; }

        public int SourceID { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public DateTime LoadDate { get; set; }
    }
}