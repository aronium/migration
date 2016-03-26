using Aronium.Migration.Models;
using System.Collections.Generic;
using System.Linq;

namespace Aronium.Migration.Commands
{
    [Command("pending", "Executes pending migration scripts")]
    public class MigratePendingCommand : MigrateCommand
    {
        private List<MigrationStatus> _migrations;

        public List<MigrationStatus> Migrations
        {
            get
            {
                if (_migrations == null)
                {
                    _migrations = new List<MigrationStatus>(GetExecutedMigrations());
                }

                return _migrations;
            }
        }
    }
}
