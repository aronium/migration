using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Aronium.Migration.Properties;
using Aronium.Migration.Models;

namespace Aronium.Migration.Commands
{
    [Command("status")]
    public class StatusCommand : DataCommandBase
    {
        public override void Run(InputArguments args)
        {
            if (this.MigrationTableExists())
            {
                var statusAray = GetMigrationStatus();
                var table = statusAray.ToStringTable(
                    s => s.ID,
                    s => s.Version,
                    s => s.Description,
                    s => s.FileName,
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

        private IEnumerable<Models.MigrationStatus> GetMigrationStatus()
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = Resources.GetMigrations;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new MigrationStatus()
                            {
                                ID = reader.GetInt32(0),
                                Version = reader.GetString(1),
                                Description = reader.GetString(2),
                                FileName = reader.GetString(3),
                                Date = reader.GetDateTime(4),
                            };
                        }
                    }
                }
            }
        }
    }
}
