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
    public class MigrateCommand : DataCommandBase
    {
        #region - Fields -

        private const string SCRIPT_SPLIT_CHAR = "###";

        #endregion

        #region - Private methods -

        private void ReadMigrationScripts()
        {
            if (!ScriptDirectoryExists())
            {
                return;
            }

            var startTime = DateTime.Now;

            Console.WriteLine(string.Format("Reading migration scripts from directory \"{0}\"", this.ScriptsDirectoryPath));

            var files = Directory.GetFiles(ScriptsDirectoryPath, "*.sql").OrderBy(x => x);

            var success = true;

            foreach (var file in files)
            {
                try
                {
                    if (ShouldExecute(file))
                    {
                        ExecuteScript(file);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    success = false;

                    break;
                }
            }

            var totalTime = DateTime.Now.Subtract(startTime);

            if (success)
            {
                Console.WriteLine(SEPARATOR_LINES);
                Console.WriteLine(string.Format(" * Migrations status:          {0}", (success) ? "SUCCESS" : "FAILURE"));
                Console.WriteLine(string.Format(" * Execution time:             {0} sec.", (int)totalTime.TotalSeconds));
                Console.WriteLine(string.Format(" * Migration completed at:     {0}", DateTime.Now));
                Console.WriteLine(string.Format(" * Current version:            {0}", double.Parse(GetCurrentVersion().ToString())));
                Console.WriteLine(SEPARATOR_LINES);
            }
        }

        private void ExecuteScript(string file)
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
                    command.Parameters.Add(new SqlParameter("@scriptName", info.Name));

                    command.ExecuteNonQuery();
                }

                #endregion
            }
        }

        private bool ShouldExecute(string fileName)
        {
            decimal currentVersion = GetCurrentVersion();

            decimal fileVersion = GetFileVersion(fileName);

            return fileVersion > currentVersion;
        }

        private bool ScriptDirectoryExists()
        {
            if (!Directory.Exists(this.ScriptsDirectoryPath))
            {
                Console.WriteLine(string.Format("ERROR Unable to locate directory on path \"{0}\"", ScriptsDirectoryPath));

                return false;
            }

            return true;
        }
        
        #endregion

        #region - Public methods -

        public override void Run(InputArguments args)
        {
            if (!IsConnectionValid())
            {
                return;
            }

            Console.WriteLine(string.Format("\nConnected to {0}/{1}\n", Config.Instance.Server, Config.Instance.Database));

            if (!ValidateMigrationTableExistance())
            {
                return;
            }

            ReadMigrationScripts();
        }

        private bool ValidateMigrationTableExistance()
        {
            if (!MigrationTableExists())
            {
                return CreateMigrationTable();
            }

            return true;
        }

        #endregion
    }
}
