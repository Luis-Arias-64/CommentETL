using System.Globalization;

namespace ETL.SPC.Application.Transform.Processing
{
    public static class TextNormalizer
    {
        public static string CleanComment(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var words = text.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', words);
        }

        // Acepta ids planos ("19") o con un prefijo de una letra ("C19", "P19", "F19")
        public static int? ParseId(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var value = raw.Trim();

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var plain))
                return plain;

            if (value.Length > 1 && char.IsLetter(value[0]) &&
                int.TryParse(value[1..], NumberStyles.Integer, CultureInfo.InvariantCulture, out var prefixed))
                return prefixed;

            return null;
        }

        public static decimal? ParseRating(int? rating, decimal min = 1.0m, decimal max = 5.0m)
        {
            if (rating is null) return null;
            var value = (decimal)rating.Value;
            return value >= min && value <= max ? value : null;
        }
    }
}
