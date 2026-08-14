using ETL.SPC.Application.Transform.Filters;
using ETL.SPC.Application.Transform.Interfaces;
using ETL.SPC.Application.Transform.Processing;
using ETL.SPC.Domain.Base;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Application.Transform
{
    public class ClientTransformer : ITransformer<ClientRaw, ClientClean>
    {
        private readonly ILogger<ClientTransformer> _logger;

        public ClientTransformer(ILogger<ClientTransformer> logger)
        {
            _logger = logger;
        }
        public IEnumerable<ClientClean> Transform(IEnumerable<ClientRaw> input)
        {
            try
            {
                var withRequiredFields = RequiredFieldsFilter.Apply(input);

                var normalized = withRequiredFields
                    .Select(ToNormalized)
                    .Where(n => n is not null)
                    .Select(n => n!.Value)
                    .ToList();

                // Mismo cliente aunque el IdCliente difiera: se considera duplicado si
                // nombre + correo (ya normalizados) coinciden. Nombre/correo no viajan
                // al DWH (DimClient ya no los tiene), pero siguen siendo la única forma
                // de detectar que dos filas son la misma persona.
                var deduped = DuplicateFilter.Apply(normalized, n => n.Fingerprint);

                return deduped.Select(n => n.Clean);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Enviando correo al administrador: Error transforming clients");
                return Enumerable.Empty<ClientClean>();
            }
        }

        private static (ClientClean Clean, string Fingerprint)? ToNormalized(ClientRaw raw)
        {
            try
            {
                var id = TextNormalizer.ParseId(raw.ClientId);
                if (id is null) return null;

                var name = TextNormalizer.CleanComment(raw.Name).ToLowerInvariant();
                var email = raw.Email.Trim().ToLowerInvariant();

                return (new ClientClean { ClientId = id.Value }, Fingerprint: $"{name}|{email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error normalizing client {raw.ClientId}: {ex.Message}");
                return null;
            }
        }
    }
}
