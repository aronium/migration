using Aronium.Migration.Models;
using System.Collections.Generic;

namespace Aronium.Migration.Commands
{
    [Command("status", "Display migration status")]
    public class StatusCommand : StatusCommandBase
    {
        protected override IEnumerable<MigrationStatus> GetMigrationStatus()
        {
            return GetExecutedMigrations();
        }
    }
}
