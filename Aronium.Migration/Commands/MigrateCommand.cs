using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using Aronium.Migration.Properties;
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

            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();

                scriptText = Regex.Replace(scriptText, @"\bGO\b", SCRIPT_SPLIT_CHAR);
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
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("ScriptName", info.Name);
                    command.Parameters.AddWithValue("Module", Module ?? Convert.DBNull);

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
