using System;
using System.IO;

namespace Aronium.Migration.Commands
{
    public abstract class MigrateCommandBase : DataCommandBase
    {
        #region - Fields -

        protected const string SCRIPT_SPLIT_CHAR = "###";

        #endregion

        private void ReadMigrationScripts()
        {
            if (!ScriptDirectoryExists())
            {
                return;
            }

            var startTime = DateTime.Now;

            Console.WriteLine(string.Format("Reading migration scripts from directory \"{0}\"", this.ScriptsDirectoryPath));

            var files = GetFiles();

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

        private bool ValidateMigrationTableExistance()
        {
            if (!MigrationTableExists())
            {
                return CreateMigrationTable();
            }

            return true;
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

        protected abstract bool ShouldExecute(string fileName);

        protected abstract void ExecuteScript(string file);

        #region - Public methods -

        public override void Run(InputArguments args)
        {
            if (!IsConnectionValid())
            {
                return;
            }

            Console.WriteLine(string.Format("\nConnected to {0}\n", Config.Instance.Database));

            if (!ValidateMigrationTableExistance())
            {
                return;
            }

            ReadMigrationScripts();
        }

        #endregion
    }
}
