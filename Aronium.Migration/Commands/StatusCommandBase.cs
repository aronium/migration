﻿using Aronium.Migration.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aronium.Migration.Commands
{
    public abstract class StatusCommandBase : DataCommandBase
    {
        protected string Module { get; set; }

        public override void Run(InputArguments args)
        {
            Module = args["module"];

            List<MigrationStatus> migrations = new List<MigrationStatus>();

            if (this.MigrationTableExists())
            {
                migrations.AddRange(GetMigrationStatus());
            }

            foreach (var status in GetFiles())
            {
                if (!migrations.Any(x => x.Version == status.Version && x.Module == status.Module))
                {
                    migrations.Add(status);
                }
            }

            if (migrations.Any())
            {
                var table = migrations.OrderBy(x => x.Version.ToVersion()).ToStringTable(
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
