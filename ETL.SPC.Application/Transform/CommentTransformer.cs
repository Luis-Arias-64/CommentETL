using ETL.SPC.Application.Transform.Filters;
using ETL.SPC.Application.Transform.Interfaces;
using ETL.SPC.Application.Transform.Processing;
using ETL.SPC.Domain.Base;

namespace ETL.SPC.Application.Transform
{
    public class CommentTransformer : ITransformer<CommentRaw, CommentClean>
    {
        public IEnumerable<CommentClean> Transform(IEnumerable<CommentRaw> input)
        {
            var withRequiredFields = RequiredFieldsFilter.Apply(input);

            var normalized = withRequiredFields
                .Select(ToClean)
                .Where(c => c is not null)
                .Select(c => c!)
                .ToList();

            // Mismo cliente + producto + fecha + comentario = misma entidad,
            // sin importar el id de origen ni de qué fuente vino.
            return DuplicateFilter.Apply(normalized, c =>
                $"{c.ClientId}|{c.ProductId}|{c.Date:yyyyMMdd}|{c.Comment.ToLowerInvariant()}");
        }

        private static CommentClean? ToClean(CommentRaw raw)
        {
            var productId = TextNormalizer.ParseId(raw.ProductId);
            if (productId is null) return null; // sin ProductId parseable no hay FK válida hacia DimProduct

            var comment = TextNormalizer.CleanComment(raw.Comment);
            var sentiment = string.IsNullOrWhiteSpace(raw.Sentiment)
                ? SentimentClassifier.Classify(comment)
                : raw.Sentiment!.Trim();

            return new CommentClean
            {
                SourceType = raw.SourceType,
                ClientId = TextNormalizer.ParseId(raw.ClientId),
                ProductId = productId.Value,
                Date = raw.Date,
                Comment = comment,
                Rating = TextNormalizer.ParseRating(raw.Rating),
                Sentiment = sentiment,
                SourceName = raw.SourceName
            };
        }
    }
}
