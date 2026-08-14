using System.Data;

namespace ETL.SPC.Application.Extract.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
