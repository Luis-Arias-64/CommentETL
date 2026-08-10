namespace CommentETL.Domain.Entities.Dimensions
{
    public class DimDate
    {
        public int DateKey { get; set; }

        public DateTime FullDate { get; set; }
        public byte Day { get; set; }
        public byte Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public byte Quarter { get; set; }
        public byte Semester { get; set; }
        public short Year { get; set; }
        public byte Week { get; set; }
    }
}