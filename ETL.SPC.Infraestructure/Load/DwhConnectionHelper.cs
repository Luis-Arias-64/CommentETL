using ETL.SPC.Application.Load.Interfaces;
using Npgsql;
using Microsoft.Extensions.Logging;

namespace ETL.SPC.Infraestructure.Load
{
    // Los 5 loaders de dimensión repiten el mismo par de pasos (abrir conexión,
    // truncar la tabla destino); se centraliza aquí para no duplicarlo.
    internal static class DwhConnectionHelper
    {
        public static async Task<NpgsqlConnection> OpenAsync(IDwhConnectionFactory factory)
        {
            try
            {
                var connection = (NpgsqlConnection)factory.CreateConnection();
                await connection.OpenAsync();
                return connection;    
            }
            catch (Exception ex)
            {
                Console.WriteLine("Correo al administrador: " + ex.Message);
                throw;
            }
            // IDwhConnectionFactory devuelve IDbConnection para no acoplar la interfaz
            // de Application a Npgsql, pero el COPY binario es una característica
            // específica de Npgsql, así que se castea aquí adentro de Infraestructure.
            
        }

        public static async Task TruncateAsync(NpgsqlConnection connection, string table)
        {
            // Full-reload: se limpia la dimensión completa antes de insertar.
            // "table" ya viene calificado con esquema en minúsculas (ej. "dim.dimclient"),
            // tal como Postgres lo guardó al no usar comillas en el DDL.
            // CASCADE es necesario: fact.FactOpinion tiene FK hacia las 5 dimensiones
            // (ver ScriptDWH.sql), y Postgres bloquea el TRUNCATE de una tabla referenciada
            // aunque la tabla que referencia esté vacía. CASCADE también vacía
            // fact.factopinion, lo cual es correcto en un full-reload de dimensiones.
            try
            {
                await using var command = new NpgsqlCommand($"TRUNCATE TABLE {table} CASCADE;", connection);
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Correo al administrador: " + ex.Message);
                throw;
            }
        }
    }
}
