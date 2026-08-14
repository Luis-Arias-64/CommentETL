using Dapper;
using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Application.Extract.Database
{
    public class DbCommentExtractor : IExtractorExternal<CommentRaw>
    {
        private readonly IDbConnectionFactory _factory;
        private readonly ILogger<DbCommentExtractor> _logger;

        public DbCommentExtractor(IDbConnectionFactory factory, ILogger<DbCommentExtractor> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<IQueryable<CommentRaw>> ExtractAsync()
        {
            try
            {
                using var conn = _factory.CreateConnection();
                const string sql = "SELECT * FROM vw_unified_feedback";
                var result = await conn.QueryAsync<CommentRaw>(sql);
                return result.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enviando correo al administrador:Error extracting comments from database");
                return Enumerable.Empty<CommentRaw>().AsQueryable(); // Return an empty result in case of error
            }
        }
    }
}
