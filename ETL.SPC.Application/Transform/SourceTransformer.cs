using ETL.SPC.Application.Transform.Filters;
using ETL.SPC.Application.Transform.Interfaces;
using ETL.SPC.Application.Transform.Processing;
using ETL.SPC.Domain.Base;

namespace ETL.SPC.Application.Transform
{
    public class SourceTransformer : ITransformer<SourceRaw, SourceClean>
    {
        public IEnumerable<SourceClean> Transform(IEnumerable<SourceRaw> input)
        {
            var withRequiredFields = RequiredFieldsFilter.Apply(input);

            var normalized = withRequiredFields
                .Select(ToClean)
                .Where(s => s is not null)
                .Select(s => s!)
                .ToList();

            return DuplicateFilter.Apply(normalized, s =>
                $"{s.SourceType.ToLowerInvariant()}|{s.LoadDate:yyyyMMdd}");
        }

        private static SourceClean? ToClean(SourceRaw raw)
        {
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
    }
}
