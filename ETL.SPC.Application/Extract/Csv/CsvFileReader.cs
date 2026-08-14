using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace ETL.SPC.Application.Extract.Csv
{
    // Wrapper delgado sobre CsvHelper: centraliza la configuración de lectura
    // (delimitador, cultura, manejo de campos con comillas/comas embebidas)
    // para que los extractores no repitan este bloque cada uno.
    internal static class CsvFileReader
    {
        public static List<T> Read<T>(string path)
        {
            // dotnet run usa como directorio de trabajo la carpeta del proyecto,
            // no bin/.../net8.0 (donde realmente se copian los CSV en el build).
            // Por eso, si la ruta es relativa, se resuelve contra la carpeta de
            // la app en ejecución (AppContext.BaseDirectory) y no contra el CWD.
            try
            {
                var resolvedPath = Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);

                if (!File.Exists(resolvedPath))
                    throw new FileNotFoundException($"No se encontró el archivo de origen: {resolvedPath}", resolvedPath);

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    TrimOptions = TrimOptions.Trim,
                };

                using var reader = new StreamReader(resolvedPath, Encoding.UTF8);
                using var csv = new CsvReader(reader, config);

                return csv.GetRecords<T>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Enviando correo al administrador: Error reading CSV file '{path}': {ex.Message}");
                return new List<T>(); // Retorna lista vacía en caso de error para que el proceso ETL continúe
            }
        }
    }
}
