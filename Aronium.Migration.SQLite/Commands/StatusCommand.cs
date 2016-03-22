using Aronium.Migration.Models;
using Aronium.Migration.SQLite.Properties;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Aronium.Migration.Commands
{
    [Command("status", "Display migration status")]
    public class StatusCommand : StatusCommandBase
    {
        protected override IEnumerable<MigrationStatus> GetMigrationStatus()
        {
            using (var connection = new SQLiteConnection(this.ConnectionString))
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
                                Module = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Date = reader.GetDateTime(5),
                            };
                        }
                    }
                }
            }
        }
    }
}
