using ETL.SPC.Application.Transform.Filters;
using ETL.SPC.Application.Transform.Interfaces;
using ETL.SPC.Application.Transform.Processing;
using ETL.SPC.Domain.Base;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Application.Transform
{
    public class SourceTransformer : ITransformer<SourceRaw, SourceClean>
    {
        private readonly ILogger<SourceTransformer> _logger;
        public SourceTransformer(ILogger<SourceTransformer> logger)
        {
            _logger = logger;
        }
        public IEnumerable<SourceClean> Transform(IEnumerable<SourceRaw> input)
        {
            try{
                var withRequiredFields = RequiredFieldsFilter.Apply(input);

                var normalized = withRequiredFields
                    .Select(ToClean)
                    .Where(s => s is not null)
                    .Select(s => s!)
                    .ToList();

                return DuplicateFilter.Apply(normalized, s =>
                    $"{s.SourceType.ToLowerInvariant()}|{s.LoadDate:yyyyMMdd}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transforming sources");
                return Enumerable.Empty<SourceClean>();
            }
        }
        private static SourceClean? ToClean(SourceRaw raw)
        {
            try{
                var id = TextNormalizer.ParseId(raw.SourceId);
                if (id is null) return null;

                if (!DateTime.TryParse(raw.LoadDate, out var loadDate)) return null;

                return new SourceClean
                {
                    SourceId = id.Value,
                    SourceType = raw.SourceType.Trim(),
                    LoadDate = loadDate
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error transforming source {raw.SourceId}: {ex.Message}");
                return null;
            }
        }
    }
}
