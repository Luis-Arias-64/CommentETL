using System.Data;
using ETL.SPC.Application.Load.Interfaces;
using Npgsql;

namespace ETL.SPC.Infraestructure.Database
{
    // Igual que PostgresConnectionFactory (fuente), pero apunta al DWH de destino.
    // Son dos conexiones/bases distintas, por eso dos factories separadas aunque
    // la implementación sea casi idéntica.
    public class PostgresDwhConnectionFactory : IDwhConnectionFactory
    {
        private readonly string _connectionString;

        public PostgresDwhConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }
}
