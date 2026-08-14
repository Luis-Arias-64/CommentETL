using System.Data;

namespace ETL.SPC.Application.Load.Interfaces
{
    public interface IDwhConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
