namespace ETL.SPC.Domain.Entities.Dimensions
{
    public class DimDate
    {
        public int DateKey { get; set; }

        public DateTime FullDate { get; set; }
        public byte Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public byte Semester { get; set; }
        public short Year { get; set; }
    }
}
