using ETL.SPC.Domain.Base;
using ETL.SPC.Domain.Entities.Dimensions;
using ETL.SPC.Domain.Entities.Fact;

namespace ETL.SPC.Application.Load
{
    // Arma las filas de staging de FactOpinion resolviendo las claves naturales de
    // cada comentario (ClientId, ProductId, SourceType, Sentiment, Date) contra las
    // mismas claves subrogadas que ya asignan DimClientLoader/DimProductLoader/
    // DimSentimentLoader al cargar sus tablas.
    //
    // IMPORTANTE: para no tocar esos Loaders ya aprobados, aquí se REPLICA su misma
    // lógica de asignación de llaves (mismo orden, mismo criterio de "distinct").
    // Si el criterio de un Loader cambia, hay que actualizar esto también — la
    // alternativa más robusta (que cada Loader devuelva su propio mapeo Id->Key en
    // vez de reconstruirlo aquí) queda como oportunidad de mejora a futuro.
    public static class FactOpinionBuilder
    {
        public static IEnumerable<FactOpinion> Build(
            IReadOnlyCollection<CommentClean> comments,
            IReadOnlyCollection<ClientClean> clients,
            IReadOnlyCollection<ProductClean> products,
            IReadOnlyCollection<SourceClean> sources,
            IReadOnlyCollection<DimSentiment> sentiments)
        {
            // Mismo criterio que DimClientLoader.
            var clientKeyByClientId = clients
                .Select(c => c.ClientId)
                .Distinct()
                .OrderBy(id => id)
                .Select((id, index) => (id, key: index + 1))
                .ToDictionary(x => x.id, x => x.key);

            // Mismo criterio que DimProductLoader.
            var productKeyByProductId = products
                .GroupBy(p => p.ProductId)
                .Select(g => g.First())
                .OrderBy(p => p.ProductId)
                .Select((p, index) => (p.ProductId, key: index + 1))
                .ToDictionary(x => x.ProductId, x => x.key);

            // Mismo criterio que DimSentimentLoader.
            var sentimentKeyBySentiment = sentiments
                .Select(s => s.Sentiment)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .Select((s, index) => (s, key: index + 1))
                .ToDictionary(x => x.s, x => x.key, StringComparer.OrdinalIgnoreCase);

            // Mismo criterio que DimSourceLoader (dedup por SourceId + orden ascendente).
            var sourcesOrdered = sources
                .GroupBy(s => s.SourceId)
                .Select(g => g.First())
                .OrderBy(s => s.SourceId)
                .ToList();

            // *** SUPUESTO A VALIDAR ***
            // fuente_datos.csv (DimSource) no tiene una llave que enlace 1 a 1 con
            // cada comentario individual — es un catálogo aparte. Como aproximación,
            // se empareja por tipo: comentarios "Web" -> la primera DimSource con
            // SourceType "Web"; cualquier otro origen (Social/Survey/DB/Api) -> la
            // primera DimSource con SourceType "CSV". Revisa si esto refleja lo que
            // esperas antes de confiar en el SourceKey del hecho.
            int? webSourceKey = ResolveSourceKey(sourcesOrdered, "Web");
            int? csvSourceKey = ResolveSourceKey(sourcesOrdered, "CSV");

            long factKey = 1;
            var rows = new List<FactOpinion>();

            foreach (var comment in comments)
            {
                // ProductKey es obligatorio (FK NOT NULL en la práctica): sin producto
                // válido, la fila no se puede insertar en fact.FactOpinion.
                if (!productKeyByProductId.TryGetValue(comment.ProductId, out var productKey))
                    continue;

                // No debería faltar: CommentTransformer siempre asigna un Sentiment
                // (clasificado por palabras clave si la fuente no lo trae).
                if (!sentimentKeyBySentiment.TryGetValue(comment.Sentiment, out var sentimentKey))
                    continue;

                int? clientKey = comment.ClientId.HasValue &&
                                  clientKeyByClientId.TryGetValue(comment.ClientId.Value, out var ck)
                    ? ck
                    : null;

                int? sourceKey = string.Equals(comment.SourceType, "Web", StringComparison.OrdinalIgnoreCase)
                    ? webSourceKey
                    : csvSourceKey;

                rows.Add(new FactOpinion
                {
                    FactOpinionKey = factKey++,
                    DateKey = int.Parse(comment.Date.ToString("yyyyMMdd")), // mismo criterio que DimDateLoader
                    ProductKey = productKey,
                    ClientKey = clientKey,
                    SourceKey = sourceKey,
                    SentimentKey = sentimentKey,
                    SatisfactionScore = comment.Rating
                });
            }

            return rows;
        }

        private static int? ResolveSourceKey(List<SourceClean> sourcesOrderedByKey, string sourceType)
        {
            var index = sourcesOrderedByKey.FindIndex(s =>
                string.Equals(s.SourceType, sourceType, StringComparison.OrdinalIgnoreCase));

            // +1 porque DimSourceLoader asigna llaves 1,2,3... en este mismo orden.
            return index >= 0 ? index + 1 : null;
        }
    }
}
