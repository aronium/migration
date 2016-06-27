using System;
using System.IO;
using System.Linq;

namespace Aronium.Migration.Commands
{
    [Command("new", "Creates new migration script with optional name provided")]
    public class CreateScriptCommand : DataCommandBase
    {
        public override void Run(InputArguments args)
        {
            var fileName = args["-new"];
            var module = args["-module"];

            if (string.IsNullOrEmpty(fileName))
            {
                Console.Write("Enter new script name: ");

                fileName = Console.ReadLine();
            }

            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrWhiteSpace(fileName))
            {
                if (!Directory.Exists(MigrationsDirectory))
                    Directory.CreateDirectory(MigrationsDirectory);

                var currentVersion = this.GetCurrentVersion(module);

                try
                {
                    if (fileName.IndexOf("__") > 0)
                    {
                        var extractedVersion = fileName.Remove(fileName.IndexOf("__")).Replace("_", ".");
                        currentVersion = decimal.Parse(extractedVersion, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return;
                }

                // Extract module related migrations
                var migrations = GetFiles().Where(x => x.Module == module).OrderBy(x => x.Version.ToVersion()).ToList();

                // If there are pending scripts in directory, get last one, and use its version number as reference
                if (migrations.Any())
                {
                    currentVersion = migrations.Last().Version.ToVersion();
                }

                // Increment major version by 1
                currentVersion = Math.Round(((decimal)((int)currentVersion + 1)), 0);

                // Create path, check module as subdirectory
                var path = Path.Combine(MigrationsDirectory, module ?? string.Empty, string.Format("{0}__{1}.sql", currentVersion.ToVersionString().Replace(".", "_"), fileName.Replace(" ", "_")));

                // Create file on specified path
                File.Create(path);

                Console.WriteLine(string.Format("New script added. Path: {0}", path));
            }
        }
    }
}
