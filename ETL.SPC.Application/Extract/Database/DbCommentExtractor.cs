using Dapper;
using ETL.SPC.Application.Extract.Interfaces;
using ETL.SPC.Domain.Base;

namespace ETL.SPC.Application.Extract.Database
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

            // El DDL real (001_schema.sql) crea todo entre comillas dobles en
            // PascalCase ("Surveys", "ClientID", "DateSurvey", "Clasification", etc.),
            // así que Postgres preserva ese casing exacto y hay que consultarlas
            // igual, entre comillas. No hay CREATE SCHEMA en el script, así que
            // las tablas quedan en "public" (el esquema por defecto).
            const string sql = @"
                SELECT
                    'Survey'                            AS ""SourceType"",
                    s.""ClientID""::text                AS ""ClientId"",
                    s.""ProductID""::text               AS ""ProductId"",
                    s.""DateSurvey""                    AS ""Date"",
                    s.""Comments""                      AS ""Comment"",
                    s.""SatisfactionPuntation""::int    AS ""Rating"",
                    s.""Clasification""                 AS ""Sentiment"",
                    t.""Name""                          AS ""SourceName""
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
