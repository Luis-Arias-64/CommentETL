using System.Data;

namespace ETL.SPC.Application.Load.Interfaces
{
    // Igual que IDbConnectionFactory de Extract, pero apuntando al DWH de
    // destino, que es una conexión/base distinta a la fuente relacional.
    public interface IDwhConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
