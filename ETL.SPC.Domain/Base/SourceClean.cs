namespace ETL.SPC.Domain.Base
{
    public class SourceClean
    {
        public int SourceId { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public DateTime LoadDate { get; set; }
    }
}
