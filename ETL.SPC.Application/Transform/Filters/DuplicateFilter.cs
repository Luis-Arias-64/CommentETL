namespace ETL.SPC.Application.Transform.Filters
{
    // Duplicado "lógico": misma huella de negocio, sin importar el PK de origen.
    // Se queda con la primera ocurrencia.
    public static class DuplicateFilter
    {
        public static IEnumerable<T> Apply<T>(IEnumerable<T> items, Func<T, string> fingerprint)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                if (seen.Add(fingerprint(item)))
                    yield return item;
            }
        }
    }
}
