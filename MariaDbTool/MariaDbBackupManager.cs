using MySql.Data.MySqlClient;
using System.Text;

namespace MariaDbTool
{
    internal class MariaDbBackupManager
    {
        private readonly string _connectionString;

        public MariaDbBackupManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task BackupAsync(string outputFilePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-- Backup generado por MariaDbBackupManager");
            sb.AppendLine("SET FOREIGN_KEY_CHECKS=0;\n");

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var tables = new List<string>();

            using (var cmd = new MySqlCommand("SHOW TABLES", connection))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                    tables.Add(reader.GetString(0));
            }

            foreach (var table in tables)
            {
                sb.AppendLine($"-- Tabla: {table}");

                // Estructura
                using (var cmd = new MySqlCommand($"SHOW CREATE TABLE `{table}`", connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        sb.AppendLine(reader.GetString(1) + ";\n");
                    }
                }

                // Datos
                using (var cmd = new MySqlCommand($"SELECT * FROM `{table}`", connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var values = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            values.Add(reader.IsDBNull(i)
                                ? "NULL"
                                : $"'{reader.GetValue(i).ToString().Replace("'", "''")}'");
                        }

                        sb.AppendLine($"INSERT INTO `{table}` VALUES ({string.Join(", ", values)});");
                    }

                    sb.AppendLine();
                }
            }

            sb.AppendLine("SET FOREIGN_KEY_CHECKS=1;");

            await File.WriteAllTextAsync(outputFilePath, sb.ToString());
            Console.WriteLine($"✅ Backup guardado en: {outputFilePath}");
        }

        public async Task RestoreAsync(string sqlFilePath)
        {
            if (!File.Exists(sqlFilePath))
            {
                Console.WriteLine("❌ Archivo SQL no encontrado.");
                return;
            }

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand();
            command.Connection = connection;

            var sb = new StringBuilder();
            var lines = await File.ReadAllLinesAsync(sqlFilePath);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("--")) continue;

                sb.AppendLine(line);

                if (trimmed.EndsWith(";"))
                {
                    command.CommandText = sb.ToString();

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("⚠️ Error en comando:");
                        Console.WriteLine(command.CommandText);
                        Console.WriteLine("➡️ Excepción: " + ex.Message);
                    }

                    sb.Clear();
                }
            }

            Console.WriteLine("✅ Restauración finalizada.");
        }
    }
}
