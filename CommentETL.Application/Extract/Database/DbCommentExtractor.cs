using Dapper;
using CommentETL.Application.Extract.Interfaces;
using CommentETL.Domain.Base;

namespace CommentETL.Application.Extract.Database
{
    public class DbCommentExtractor : IExtractor<CommentRaw>
    {
        private readonly IDbConnectionFactory _factory;

        public DbCommentExtractor(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<CommentRaw>> ExtractAsync()
        {
            using var conn = _factory.CreateConnection();

            // La DB no tiene una tabla única de comentarios: son 3 tablas paralelas
            // (Surveys, WebReviews, SocialComments), cada una enlazada a DataSources -> TypeSource.
            // Se unifican con UNION ALL para devolver un solo IEnumerable<CommentRaw>,
            // igual que hacen los extractores CSV.
            const string sql = @"
                SELECT
                    'Survey'                       AS ""SourceType"",
                    s.""ClientID""::text            AS ""ClientId"",
                    s.""ProductID""::text           AS ""ProductId"",
                    s.""DateSurvey""                AS ""Date"",
                    s.""Comments""                  AS ""Comment"",
                    s.""SatisfactionPuntation""::int AS ""Rating"",
                    s.""Clasification""             AS ""Sentiment"",
                    t.""Name""                      AS ""SourceName""
                FROM ""Surveys"" s
                LEFT JOIN ""DataSources"" d ON d.""DataSourceID"" = s.""SurveysSourceID""
                LEFT JOIN ""TypeSource""  t ON t.""KindSourceID"" = d.""KindSourceID""

                UNION ALL

                SELECT
                    'Web',
                    w.""ClientID""::text,
                    w.""ProductID""::text,
                    w.""DateReviews"",
                    w.""Coments"",
                    w.""Ratings""::int,
                    NULL::text,
                    t.""Name""
                FROM ""WebReviews"" w
                LEFT JOIN ""DataSources"" d ON d.""DataSourceID"" = w.""DataSourceID""
                LEFT JOIN ""TypeSource""  t ON t.""KindSourceID"" = d.""KindSourceID""

                UNION ALL

                SELECT
                    'Social',
                    sc.""ClientID""::text,
                    sc.""ProductID""::text,
                    sc.""DateComent"",
                    sc.""Comment"",
                    NULL::int,
                    NULL::text,
                    t.""Name""
                FROM ""SocialComments"" sc
                LEFT JOIN ""DataSources"" d ON d.""DataSourceID"" = sc.""CommentsSoruceID""
                LEFT JOIN ""TypeSource""  t ON t.""KindSourceID"" = d.""KindSourceID""";

            var result = await conn.QueryAsync<CommentRaw>(sql);

            return result;
        }
    }
}