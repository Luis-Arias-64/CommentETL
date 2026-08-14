using System.Globalization;
using ETL.SPC.Domain.Base;
using ETL.SPC.Domain.Entities.Dimensions;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Application.Load
{
    public static class DimensionBuilder
    {
        public static IEnumerable<DimDate> BuildDates(IEnumerable<CommentClean> comments)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Correo al administrador: Error building DimDate: {ex.Message}");
                return Enumerable.Empty<DimDate>();
            }
        }

        public static IEnumerable<DimSentiment> BuildSentiments(IEnumerable<CommentClean> comments)
        {
            try
            {
                return comments
                    .Select(c => c.Sentiment)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select(s => new DimSentiment { Sentiment = s });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Correo al administrador: Error building DimSentiment: {ex.Message}");
                return Enumerable.Empty<DimSentiment>();
            }
        }
    }
}
