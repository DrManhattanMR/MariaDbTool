using MariaDbTool;
using System;
using System.IO;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("❌ Uso incorrecto.");
            Console.WriteLine("Ejemplos:");
            Console.WriteLine("  dotnet run -- --backup [ruta/opcional]");
            Console.WriteLine("  dotnet run -- --restore ruta");
            return;
        }

        string action = args[0];
        string filePath = args.Length >= 2 ? args[1] : null;

        var connectionString = "server=localhost;port=3306;user=root;password=1234;database=midb"; // Ajusta aquí
        var manager = new MariaDbBackupManager(connectionString);

        if (action == "--backup")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                string folder = "respaldos";
                Directory.CreateDirectory(folder);

                filePath = Path.Combine(folder, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql");
                Console.WriteLine($"[INFO] No se proporcionó ruta. Se usará archivo generado automáticamente: {filePath}");
            }

            await manager.BackupAsync(filePath);
        }
        else if (action == "--restore")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("❌ Para restaurar es obligatorio especificar la ruta del archivo SQL.");
                return;
            }

            await manager.RestoreAsync(filePath);
        }
        else
        {
            Console.WriteLine("❌ Acción no reconocida. Usa --backup o --restore");
        }
    }
}
