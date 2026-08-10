using System.Data;
using CommentETL.Application.Extract.Database;
using Npgsql;

namespace CommentETL.Infraestructure.Database
{
    // Implementación concreta de IDbConnectionFactory para la base relacional de origen
    // (PostgreSQL). Vive en Infraestructure porque es un detalle de infraestructura;
    // Application solo conoce la interfaz.
    public class PostgresConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public PostgresConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }
}
