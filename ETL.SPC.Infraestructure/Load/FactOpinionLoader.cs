using ETL.SPC.Application.Load.Interfaces;
using ETL.SPC.Domain.Entities.Fact;
using Npgsql;
using NpgsqlTypes;

namespace ETL.SPC.Infraestructure.Load
{
    public class FactOpinionLoader : ILoader<FactOpinion>
    {
        private readonly IDwhConnectionFactory _factory;

        public FactOpinionLoader(IDwhConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task LoadAsync(IEnumerable<FactOpinion> items)
        {
            await using var connection = await DwhConnectionHelper.OpenAsync(_factory);

            // La tabla de staging se crea desde código (según lo pedido: crearla
            // directamente en la base complica el flujo del proyecto). Se asume
            // esquema "public" (por defecto) porque el procedure fact.sp_cargar_fact_opinion
            // la referencia sin calificar de esquema ("FROM tmp_fact_opinion", no
            // "fact.tmp_fact_opinion") — si tu search_path resuelve distinto, ajusta
            // el nombre calificado aquí.
            // Tipos y nombres de columna tomados de creacionProcedure.sql (no me
            // llegó el CREATE TABLE del staging, solo el procedure que lo consume).
            const string createStaging = @"
                CREATE TABLE IF NOT EXISTS tmp_fact_opinion (
                    tem_factopinionkey    BIGINT PRIMARY KEY,
                    tem_datekey           INT,
                    tem_productkey        INT,
                    tem_clientkey         INT,
                    tem_sourcekey         INT,
                    tem_sentimentkey      INT,
                    tem_satisfactionscore DECIMAL(5,2)
                );";

            await using (var create = new NpgsqlCommand(createStaging, connection))
                await create.ExecuteNonQueryAsync();

            // Se limpia el staging antes de cargar, por si quedó algo de una corrida
            // anterior que no haya llegado a completar el CALL al procedure.
            await DwhConnectionHelper.TruncateAsync(connection, "tmp_fact_opinion");

            await using (var writer = await connection.BeginBinaryImportAsync(
                "COPY tmp_fact_opinion (tem_factopinionkey, tem_datekey, tem_productkey, tem_clientkey, tem_sourcekey, tem_sentimentkey, tem_satisfactionscore) FROM STDIN (FORMAT BINARY)"))
            {
                foreach (var fact in items)
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(fact.FactOpinionKey, NpgsqlDbType.Bigint);
                    await writer.WriteAsync(fact.DateKey, NpgsqlDbType.Integer);
                    await writer.WriteAsync(fact.ProductKey, NpgsqlDbType.Integer);

                    if (fact.ClientKey.HasValue)
                        await writer.WriteAsync(fact.ClientKey.Value, NpgsqlDbType.Integer);
                    else
                        await writer.WriteNullAsync();

                    if (fact.SourceKey.HasValue)
                        await writer.WriteAsync(fact.SourceKey.Value, NpgsqlDbType.Integer);
                    else
                        await writer.WriteNullAsync();

                    await writer.WriteAsync(fact.SentimentKey, NpgsqlDbType.Integer);

                    if (fact.SatisfactionScore.HasValue)
                        await writer.WriteAsync(fact.SatisfactionScore.Value, NpgsqlDbType.Numeric);
                    else
                        await writer.WriteNullAsync();
                }

                await writer.CompleteAsync();
            }

            // El procedure valida que el staging tenga filas, inserta hacia
            // fact.FactOpinion (ON CONFLICT DO NOTHING) y trunca el staging al final.
            await using var call = new NpgsqlCommand("CALL fact.sp_cargar_fact_opinion();", connection);
            await call.ExecuteNonQueryAsync();
        }
    }
}
