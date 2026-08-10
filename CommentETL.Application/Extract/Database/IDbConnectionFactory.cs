using System.Data;

namespace CommentETL.Application.Extract.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
