using CommentETL.Application.Extract.Interfaces;
using CommentETL.Domain.Base;

namespace CommentETL.Application.Extract.Csv
{
    public class SourceExtractor : IExtractor<SourceRaw>
    {
        private readonly string _path;

        public SourceExtractor(string path)
        {
            _path = path;
        }

        public Task<IEnumerable<SourceRaw>> ExtractAsync()
        {
            var rows = CsvFileReader.Read<SourceCsvRow>(_path);

            var result = rows.Select(r => new SourceRaw
            {
                SourceId = r.IdFuente,
                SourceType = r.TipoFuente,
                LoadDate = r.FechaCarga
            });

            return Task.FromResult(result);
        }
    }
}
