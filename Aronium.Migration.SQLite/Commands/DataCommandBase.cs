using Aronium.Migration.Models;
using Aronium.Migration.SQLite.Properties;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Aronium.Migration.Commands
{
    public abstract class DataCommandBase : CommandBase
    {
        #region - Fields -

        private string _connectionString;

        #endregion

        #region - Properties -
        /// <summary>
        /// Gets connection string.
        /// </summary>
        protected string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    _connectionString = CreateConnectionString();
                }
                return _connectionString;
            }
        } 
        #endregion

        #region - Protected methods -

        /// <summary>
        /// Gets a value indicating whether connection is valid.
        /// </summary>
        /// <returns>True if connection is valid, otherwise false.</returns>
        protected bool IsConnectionValid()
        {
            using (var conn = new SQLiteConnection(this.ConnectionString))
            {
                try
                {
                    conn.Open();

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("ERROR {0}", ex.Message));

                    return false;
                }
            }
        }

        /// <summary>
        /// Creates connection string.
        /// </summary>
        /// <returns>Connection string.</returns>
        protected string CreateConnectionString()
        {
            var builder = new SQLiteConnectionStringBuilder();
            builder.FailIfMissing = true;
            builder.DataSource = Config.Instance.Database;
            builder.Version = 3;

            return builder.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether migration table exists.
        /// </summary>
        /// <returns>True if migration table exists, otherwise false.</returns>
        protected bool MigrationTableExists()
        {
            bool migrationTableExists = false;

            try
            {
                using (var connection = new SQLiteConnection(this.ConnectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = Resources.MigrationTableExists;
                        SQLiteDataReader reader = command.ExecuteReader();

                        reader.Read();
                        if (reader.HasRows)
                        {
                            migrationTableExists = (long)reader[0] > 0;
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return migrationTableExists;
        }

        /// <summary>
        /// Creates migration table.
        /// </summary>
        /// <returns>True if table is created successfully, otherwise false.</returns>
        protected bool CreateMigrationTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(this.ConnectionString))
                {
                    if (!MigrationTableExists())
                    {
                        connection.Open();

                        Console.WriteLine("Creating Migrations table...");
                        Console.WriteLine();

                        using (SQLiteCommand command = connection.CreateCommand())
                        {
                            command.CommandText = Resources.CreateMigrationsTable;
                            command.ExecuteNonQuery();
                        }

                        Console.WriteLine("Migrations table created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Migrations table exists.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets current database version, the max version number from database.
        /// </summary>
        /// <returns>Current database version number.</returns>
        protected decimal GetCurrentVersion()
        {
            decimal currentVersion = 0;

            if (!MigrationTableExists())
                CreateMigrationTable();

            using (var connection = new SQLiteConnection(this.ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = Resources.GetCurrentVersion;

                    using (var reader = command.ExecuteReader(System.Data.CommandBehavior.SingleResult | System.Data.CommandBehavior.CloseConnection))
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            var record = reader[0];
                            if (record != Convert.DBNull)
                                currentVersion = Convert.ToDecimal(record);
                        }

                        reader.Close();
                    }
                }
            }

            return currentVersion;
        }
        
        /// <summary>
        /// Gets executed migrations.
        /// </summary>
        /// <returns>List of executed migrations.</returns>
        protected IEnumerable<MigrationStatus> GetExecutedMigrations()
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

        #endregion
    }
}
