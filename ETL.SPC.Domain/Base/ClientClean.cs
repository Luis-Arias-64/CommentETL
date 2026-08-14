namespace ETL.SPC.Domain.Base
{
    // Refleja el nuevo DimClient del DWH: solo se necesita la clave natural.
    public class ClientClean
    {
        public int ClientId { get; set; }
    }
}
