using ETL.SPC.Domain.Base;

namespace ETL.SPC.Application.Transform.Filters
{
    // Descarta registros sin los campos obligatorios para el DWH.
    // Se aplica sobre los datos crudos, antes de normalizar nada.
    public static class RequiredFieldsFilter
    {
        public static IEnumerable<CommentRaw> Apply(IEnumerable<CommentRaw> comments) =>
            comments.Where(c =>
                !string.IsNullOrWhiteSpace(c.ProductId) &&
                !string.IsNullOrWhiteSpace(c.Comment) &&
                c.Date != DateTime.MinValue); // MinValue = falló el parseo de fecha en Extract

        public static IEnumerable<ClientRaw> Apply(IEnumerable<ClientRaw> clients) =>
            clients.Where(c =>
                !string.IsNullOrWhiteSpace(c.ClientId) &&
                !string.IsNullOrWhiteSpace(c.Name) &&
                !string.IsNullOrWhiteSpace(c.Email) && c.Email.Contains('@'));

        public static IEnumerable<ProductRaw> Apply(IEnumerable<ProductRaw> products) =>
            products.Where(p =>
                !string.IsNullOrWhiteSpace(p.ProductId) &&
                !string.IsNullOrWhiteSpace(p.Name) &&
                !string.IsNullOrWhiteSpace(p.Category));

        public static IEnumerable<SourceRaw> Apply(IEnumerable<SourceRaw> sources) =>
            sources.Where(s =>
                !string.IsNullOrWhiteSpace(s.SourceId) &&
                !string.IsNullOrWhiteSpace(s.SourceType) &&
                !string.IsNullOrWhiteSpace(s.LoadDate));
    }
}
