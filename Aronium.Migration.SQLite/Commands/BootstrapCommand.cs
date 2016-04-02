using System;
using System.Data.SQLite;
using System.IO;

namespace Aronium.Migration.Commands
{
    [Command("bootstrap", "Initializes database and configuration")]
    internal class BootstrapCommand : DataCommandBase
    {
        public override void Run(InputArguments args)
        {
            Console.WriteLine(SEPARATOR_LINES);
            Console.WriteLine("Begin bootstrap process.");

            Console.Write("Database file: ");
            Config.Instance.Database = Console.ReadLine();

            var fullPath = Path.GetFullPath(Config.Instance.Database);

            Console.WriteLine($"Selected file: {fullPath}");

            if (!File.Exists(fullPath))
            {
                bool isAnsweredCorrectly = false;

                while (!isAnsweredCorrectly)
                {
                    Console.Write("Specified file do not exists. Do you want to create new database file? Y/N ");

                    var input = Console.ReadLine();

                    isAnsweredCorrectly = input.StartsWith("y", StringComparison.OrdinalIgnoreCase) || input.StartsWith("n", StringComparison.OrdinalIgnoreCase);

                    if (input.Equals("y", StringComparison.OrdinalIgnoreCase) || input.Equals("yes", StringComparison.OrdinalIgnoreCase))
                    {
                        SQLiteConnection.CreateFile(fullPath);
                    }
                }
            }

            Config.Save();

            Console.WriteLine();
            Console.WriteLine(SEPARATOR_LINES);

            if (this.IsConnectionValid())
            {
                Console.WriteLine("Bootstrap completed.");
            }
            
            Console.WriteLine(SEPARATOR_LINES);
        }
    }
}
