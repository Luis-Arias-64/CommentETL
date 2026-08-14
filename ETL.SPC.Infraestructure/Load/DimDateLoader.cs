using ETL.SPC.Application.Load.Interfaces;
using ETL.SPC.Domain.Entities.Dimensions;
using NpgsqlTypes;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Infraestructure.Load
{
    public class DimDateLoader : ILoader<DimDate>
    {
        private readonly ILogger<DimDateLoader> _logger;
        private readonly IDwhConnectionFactory _factory;

        public DimDateLoader(IDwhConnectionFactory factory, ILogger<DimDateLoader> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task LoadAsync(IEnumerable<DimDate> items)
        {
            try{
                await using var connection = await DwhConnectionHelper.OpenAsync(_factory);
                await DwhConnectionHelper.TruncateAsync(connection, "dim.dimdate");

                var distinctDates = items
                    .GroupBy(d => d.DateKey)
                    .Select(g => g.First())
                    .OrderBy(d => d.DateKey)
                    .ToList();

                await using var writer = await connection.BeginBinaryImportAsync(
                    "COPY dim.dimdate (datekey, fulldate, month, monthname, semester, year) FROM STDIN (FORMAT BINARY)");

                foreach (var date in distinctDates)
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(date.DateKey, NpgsqlDbType.Integer);
                    await writer.WriteAsync(date.FullDate, NpgsqlDbType.Date);
                    await writer.WriteAsync((short)date.Month, NpgsqlDbType.Smallint); // Postgres no tiene tinyint
                    await writer.WriteAsync(date.MonthName, NpgsqlDbType.Varchar);
                    await writer.WriteAsync((short)date.Semester, NpgsqlDbType.Smallint);
                    await writer.WriteAsync(date.Year, NpgsqlDbType.Smallint);
                }

                await writer.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dates");
                Console.WriteLine($"Correo al administrador: {ex.Message}");
            }
        }
    }
}
