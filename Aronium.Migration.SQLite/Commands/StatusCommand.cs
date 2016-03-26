using Aronium.Migration.Models;
using System.Collections.Generic;

namespace Aronium.Migration.Commands
{
    [Command("status", "Display migration status")]
    public class StatusCommand : StatusCommandBase
    {
        protected override IEnumerable<MigrationStatus> GetMigrationStatus()
        {
            return GetExecutedMigrations(Module);
        }

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
