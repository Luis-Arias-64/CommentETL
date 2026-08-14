using System.Globalization;
using ETL.SPC.Domain.Base;
using ETL.SPC.Domain.Entities.Dimensions;

namespace ETL.SPC.Application.Load
{
    // DimDate y DimSentiment no vienen de un CSV/tabla propia: se derivan de
    // los valores distintos que quedaron en los comentarios ya transformados.
    public static class DimensionBuilder
    {
        public static IEnumerable<DimDate> BuildDates(IEnumerable<CommentClean> comments)
        {
            var culture = CultureInfo.GetCultureInfo("es-ES");

            return comments
                .Select(c => c.Date.Date)
                .Distinct()
                .Select(date => new DimDate
                {
                    DateKey = int.Parse(date.ToString("yyyyMMdd")),
                    FullDate = date,
                    Month = (byte)date.Month,
                    MonthName = culture.DateTimeFormat.GetMonthName(date.Month),
                    Semester = (byte)(date.Month <= 6 ? 1 : 2),
                    Year = (short)date.Year
                });
        }

        public static IEnumerable<DimSentiment> BuildSentiments(IEnumerable<CommentClean> comments)
        {
            return comments
                .Select(c => c.Sentiment)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(s => new DimSentiment { Sentiment = s });
        }
    }
}
