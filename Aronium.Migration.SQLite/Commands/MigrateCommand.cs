using Aronium.Migration.Models;
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

        protected override void Execute(MigrationStatus migration)
        {
            Console.WriteLine();
            Console.WriteLine(string.Format("========== Applying \"{0}\" ==========", migration.FileName));
            Console.WriteLine();

            var scriptText = File.ReadAllText(migration.Path);

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

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("version", migration.Version);
                    command.Parameters.AddWithValue("description", migration.Description);
                    command.Parameters.AddWithValue("module", migration.Module ?? Module);
                    command.Parameters.AddWithValue("fileName", migration.FileName);

                    command.ExecuteNonQuery();
                }

                #endregion
            }
        }

        #endregion

        public override void Run(InputArguments args)
        {
            var dir = args["dir"];
            var database = args["database"];

            if (!string.IsNullOrEmpty(dir))
                MigrationsDirectory = dir;

            if (!string.IsNullOrEmpty(database))
                Database = database;

            base.Run(args);
        }
    }
}
