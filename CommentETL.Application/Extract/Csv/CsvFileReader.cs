using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace CommentETL.Application.Extract.Csv
{
    // Wrapper delgado sobre CsvHelper: centraliza la configuración de lectura
    // (delimitador, cultura, manejo de campos con comillas/comas embebidas)
    // para que los extractores no repitan este bloque cada uno.
    internal static class CsvFileReader
    {
        public static List<T> Read<T>(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"No se encontró el archivo de origen: {path}", path);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim,
            };

            using var reader = new StreamReader(path, Encoding.UTF8);
            using var csv = new CsvReader(reader, config);

            return csv.GetRecords<T>().ToList();
        }
    }
}
