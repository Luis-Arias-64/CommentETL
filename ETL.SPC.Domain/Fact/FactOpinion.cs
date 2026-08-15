namespace ETL.SPC.Domain.Entities.Fact
{
    public class FactOpinion
    {
        public long FactOpinionKey { get; set; }
        public int DateKey { get; set; }
        public int ProductKey { get; set; }
        public int? ClientKey { get; set; }   // nullable: hay comentarios sin cliente identificado
        public int? SourceKey { get; set; }   // nullable: ver nota de mapeo heurístico en FactOpinionBuilder
        public int SentimentKey { get; set; }
        public decimal? SatisfactionScore { get; set; }
    }
}
