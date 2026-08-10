namespace CommentETL.Domain.Base
{
    // Representa una fila cruda de clients.csv, sin validar ni transformar.
    // La normalización (parseo de tipos, validaciones) se hace en la fase de Transform.
    public class ClientRaw
    {
        public string ClientId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
