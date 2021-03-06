﻿using Aronium.Migration.Models;
using Aronium.Migration.Properties;
using System;
using System.Data.SqlClient;
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
                    command.Parameters.AddWithValue("ScriptName", migration.FileName);
                    command.Parameters.AddWithValue("Module", migration.Module ?? Convert.DBNull);

                    command.ExecuteNonQuery();
                }

                #endregion
            }
        }

        #endregion
    }
}
