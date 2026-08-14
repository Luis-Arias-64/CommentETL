namespace ETL.SPC.Application.Transform.Processing
{
    // Clasificación simple por palabras clave, para cuando la fuente no trae
    // sentimiento (Social y Web no tienen columna de clasificación).
    public static class SentimentClassifier
    {
        private static readonly string[] PositiveWords =
        {
            "excelente", "genial", "encanta", "increíble", "increible", "perfecto",
            "recomiendo", "recomendable", "satisfecho", "satisfecha", "rápido", "rapido",
            "buena", "bueno", "mejor", "gran", "cumple"
        };

        private static readonly string[] NegativeWords =
        {
            "malo", "mala", "pésimo", "pesimo", "terrible", "decepcionado", "decepcionada",
            "lento", "defectuoso", "rompió", "rompio", "queja", "problema", "horrible",
            "insatisfecho", "no volvería", "no volveria", "no recomiendo"
        };

        public static string Classify(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return "Neutra";

            var text = comment.ToLowerInvariant();
            var positives = PositiveWords.Count(text.Contains);
            var negatives = NegativeWords.Count(text.Contains);

            if (positives == negatives) return "Neutra";
            return positives > negatives ? "Positiva" : "Negativa";
        }
    }
}
