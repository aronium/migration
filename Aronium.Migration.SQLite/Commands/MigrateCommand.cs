using Aronium.Migration.SQLite.Properties;
using System;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;

namespace Aronium.Migration.Commands
{
    [Command("up", "Executes migration scripts")]
    public class MigrateCommand : MigrateCommandBase
    {
        #region - Private methods -

        protected override void ExecuteScript(string file)
        {
            var info = new FileInfo(file);

            Console.WriteLine();
            Console.WriteLine(string.Format("========== Applying \"{0}\" ==========", info.Name));
            Console.WriteLine();

            var scriptText = File.ReadAllText(file);

            using (var connection = new SQLiteConnection(this.ConnectionString))
            {
                connection.Open();

                scriptText = Regex.Replace(scriptText, @";", SCRIPT_SPLIT_CHAR);
                string[] commands = scriptText.Split(new string[] { SCRIPT_SPLIT_CHAR }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var commandText in commands)
                {
                    if (!string.IsNullOrEmpty(commandText.Trim()))
                    {
                        Console.WriteLine(commandText);
                        Console.WriteLine();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = commandText;
                            command.CommandTimeout = 0;

                            command.ExecuteNonQuery();
                        }
                    }
                }

                #region " Log changeset "

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = Resources.LogMigration;

                    var status = ParseFileName(file);

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("version", status.Version);
                    command.Parameters.AddWithValue("description", status.Description);
                    command.Parameters.AddWithValue("module", status.Module);
                    command.Parameters.AddWithValue("fileName", status.FileName);

                    command.ExecuteNonQuery();
                }

                #endregion
            }
        }

        protected override bool ShouldExecute(string fileName)
        {
            decimal currentVersion = GetCurrentVersion();

            decimal fileVersion = GetFileVersion(fileName);

            return fileVersion > currentVersion;
        }
        
        #endregion
    }
}
