using Aronium.Migration.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Aronium.Migration.Commands
{
    public abstract class StatusCommandBase : DataCommandBase
    {
        public override void Run(InputArguments args)
        {
            if (this.MigrationTableExists())
            {
                var migrations = GetMigrationStatus().ToList();

                foreach(var file in GetFiles())
                {
                    if (!migrations.Any(x => x.Version == GetFileVersion(file).ToString("0.0#####", System.Globalization.CultureInfo.InvariantCulture)))
                    {
                        migrations.Add(ParseFileName(file));
                    }
                }

                var table = migrations.OrderBy(x=>x.Version.ToVersion()).ToStringTable(
                    s => s.ID,
                    s => s.Version,
                    s => s.Description,
                    s => s.FileName,
                    s => s.Module,
                    s => s.Date
                );

                Console.WriteLine(table);
            }
            else
            {
                Console.WriteLine(SEPARATOR_LINES);
                Console.WriteLine(" * NO MIGRATIONS FOUND");
                Console.WriteLine(SEPARATOR_LINES);
            }
        }

        protected abstract IEnumerable<MigrationStatus> GetMigrationStatus();
    }
}
