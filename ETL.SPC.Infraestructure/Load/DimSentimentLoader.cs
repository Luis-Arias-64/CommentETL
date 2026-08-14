using ETL.SPC.Application.Load.Interfaces;
using ETL.SPC.Domain.Entities.Dimensions;
using NpgsqlTypes;

namespace ETL.SPC.Infraestructure.Load
{
    public class DimSentimentLoader : ILoader<DimSentiment>
    {
        private readonly IDwhConnectionFactory _factory;

        public DimSentimentLoader(IDwhConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task LoadAsync(IEnumerable<DimSentiment> items)
        {
            await using var connection = await DwhConnectionHelper.OpenAsync(_factory);
            await DwhConnectionHelper.TruncateAsync(connection, "dim.dimsentiment");

            var distinctSentiments = items
                .Select(s => s.Sentiment)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            await using var writer = await connection.BeginBinaryImportAsync(
                "COPY dim.dimsentiment (sentimentkey, sentiment) FROM STDIN (FORMAT BINARY)");

            var surrogateKey = 1;
            foreach (var sentiment in distinctSentiments)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(surrogateKey, NpgsqlDbType.Integer);
                await writer.WriteAsync(sentiment, NpgsqlDbType.Varchar);
                surrogateKey++;
            }

            await writer.CompleteAsync();
        }
    }
}
