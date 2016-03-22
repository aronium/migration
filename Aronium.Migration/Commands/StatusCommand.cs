using Aronium.Migration.Models;
using Aronium.Migration.Properties;
using System.Collections.Generic;
using System.Data.SqlClient;

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
