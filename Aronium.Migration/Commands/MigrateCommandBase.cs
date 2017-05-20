using Aronium.Migration.Models;
using System;
using System.IO;
using System.Linq;

namespace Aronium.Migration.Commands
{
    public abstract class MigrateCommandBase : DataCommandBase
    {
        #region - Fields -

        protected const string SCRIPT_SPLIT_CHAR = "###";

        /// <summary>
        /// Gets or sets module.
        /// </summary>
        protected string Module { get; set; }

        #endregion

        private void ReadMigrationScripts()
        {
            if (!ScriptDirectoryExists())
            {
                return;
            }

            var startTime = DateTime.Now;

            var files = GetFiles();

            var success = true;

            foreach (var migration in files)
            {
                try
                {
                    if (ShouldExecute(migration))
                    {
                        Execute(migration);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);

                    success = false;

                    Environment.ExitCode = 10;

                    break;
                }
            }

            var totalTime = DateTime.Now.Subtract(startTime);

            if (success)
            {
                Console.WriteLine(SEPARATOR_LINES);
                Console.WriteLine(string.Format(" * Migrations status:          {0}", (success) ? "SUCCESS" : "FAILURE"));
                Console.WriteLine(string.Format(" * Execution time:             {0} ms", (int)totalTime.TotalMilliseconds));
                Console.WriteLine(string.Format(" * Migration completed at:     {0}", DateTime.Now));
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
            if (!Directory.Exists(this.MigrationsDirectory))
            {
                Console.WriteLine(string.Format("ERROR Unable to locate directory \"{0}\"", MigrationsDirectory));

                return false;
            }

            return true;
        }

        protected virtual bool ShouldExecute(MigrationStatus migration)
        {
            decimal currentVersion = GetCurrentVersion(migration.Module);

            return migration.Version.ToVersion() > currentVersion;
        }

        protected abstract void Execute(MigrationStatus migration);

        #region - Public methods -

        public override void Run(InputArguments args)
        {
            if (!IsConnectionValid())
            {
                return;
            }

            Module = args["module"];

            Console.WriteLine(string.Format("\nConnected to {0}\n", Database));

            if (!ValidateMigrationTableExistance())
            {
                return;
            }

            ReadMigrationScripts();
        }

        #endregion
    }
}
