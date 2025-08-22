// Program.cs
using MariaDbTool;
using System;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("❌ Debes indicar una acción: --backup o --restore");
            return;
        }

        var connectionString = "server=localhost;port=0000;user=username;password=passworduser;database=databasename"; // Cambia por tus valores
        var manager = new MariaDbBackupManager(connectionString);

        if (args[0] == "--backup")
        {
            string backupPath = "backup.sql"; // Puedes personalizar el nombre
            await manager.BackupAsync(backupPath);
        }
        else if (args[0] == "--restore")
        {
            string restorePath = "backup.sql"; // Asegúrate de que existe
            await manager.RestoreAsync(restorePath);
        }
        else
        {
            Console.WriteLine("❌ Opción no reconocida. Usa --backup o --restore");
        }
    }
}
