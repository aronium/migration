using Aronium.Migration.SQLite.Properties;
using System;
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

        #region - Private methods -

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
                            migrationTableExists = (int)reader[0] > 0;
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
                                currentVersion = (decimal)record;
                        }

                        reader.Close();
                    }
                }
            }

            return currentVersion;
        }
        
        /// <summary>
        /// Gets script version from file name.
        /// </summary>
        /// <param name="fileName">File name to parse.</param>
        /// <returns>Script version.</returns>
        protected decimal GetFileVersion(string fileName)
        {
            fileName = new FileInfo(fileName).Name;

            var extractedVersion = fileName.Remove(fileName.IndexOf("__")).Replace("_", ".");

            return decimal.Parse(extractedVersion);
        }

        #endregion
    }
}
