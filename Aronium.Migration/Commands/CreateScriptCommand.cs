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

            if (string.IsNullOrEmpty(fileName))
            {
                Console.Write("Enter new script name: ");

                fileName = Console.ReadLine();
            }

            if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrWhiteSpace(fileName))
            {
                if (!Directory.Exists(ScriptsDirectoryPath))
                    Directory.CreateDirectory(ScriptsDirectoryPath);
                
                var currentVersion = this.GetCurrentVersion();

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

                // Check if there are any pending scripts
                var pendingScripts = from file in Directory.GetFiles(ScriptsDirectoryPath) where GetFileVersion(file) > currentVersion select file;
                // If there are pending scripts in directory, get last one, and use its version number as reference
                if (pendingScripts.Any())
                {
                    currentVersion = GetFileVersion(pendingScripts.Last());
                }

                // Increment major version by 1
                currentVersion = Math.Round(((decimal)((int)currentVersion + 1)), 0);

                // Create string version using migration file name rules
                var stringVersion = currentVersion.ToString("0.0#####", System.Globalization.CultureInfo.InvariantCulture).Replace(".", "_");

                // Create path
                var path = Path.Combine(ScriptsDirectoryPath, string.Format("{0}__{1}.sql", stringVersion, fileName.Replace(" ", "_")));

                // Create file on specified path
                File.Create(path);

                Console.WriteLine(string.Format("New script added. Path: {0}", path));
            }
        }
    }
}
