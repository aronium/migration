using Aronium.Migration.Properties;
using System;
using System.Linq;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using Aronium.Migration.Models;

namespace Aronium.Migration.Commands
{
    public abstract class DataCommandBase : CommandBase
    {
        #region - Fields -

        private string _connectionString;
        private string _database;

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

        /// <summary>
        /// Gets or sets database.
        /// </summary>
        protected string Database
        {
            get
            {
                return _database ?? Config.Instance.Database;
            }
            set
            {
                _database = value;
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
            using (var conn = new SqlConnection(this.ConnectionString))
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
            SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder();
            cb.DataSource = Config.Instance.Server;
            cb.InitialCatalog = Config.Instance.Database;
            cb.MultipleActiveResultSets = true;
            cb.UserID = Config.Instance.Username;
            cb.Password = Config.Instance.Password;
            cb.Pooling = true;

            // No timeout
            cb.ConnectTimeout = 0;

            return cb.ToString();
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
                using (var connection = new SqlConnection(this.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = Resources.MigrationTableExists;
                        SqlDataReader reader = command.ExecuteReader();

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
                using (var connection = new SqlConnection(this.ConnectionString))
                {
                    if (!MigrationTableExists())
                    {
                        connection.Open();

                        Console.WriteLine("Creating Migrations table...");
                        Console.WriteLine();

                        using (SqlCommand command = connection.CreateCommand())
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
        /// <param name="module">Module for which current version is returned.</param>
        /// <returns>Current database version number.</returns>
        protected decimal GetCurrentVersion(string module)
        {
            decimal currentVersion = 0;

            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = Resources.GetCurrentVersion;

                    command.Parameters.AddWithValue("Module", module ?? Convert.DBNull);

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
        /// Gets executed migrations.
        /// </summary>
        /// <param name="module">Module for which migrations are selected. If not set, all migrations are returned.</param>
        /// <returns>List of executed migrations.</returns>
        protected IEnumerable<MigrationStatus> GetExecutedMigrations(string module = null)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = Resources.GetMigrations;

                    command.Parameters.AddWithValue("Module", module ?? Convert.DBNull);

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

        /// <summary>
        /// Attempts to read connection properties from input arguments and sets Config properties accordingly.
        /// <para>Input arguments must contain the following keys: -server SERVER_NAME -database DATABASE_NAME -username USERNAME -password PASSWORD</para>
        /// </summary>
        /// <param name="args">Args to set connection properties from.</param>
        /// <returns>True if all properties are available and set, otherwise false.</returns>
        internal static bool TrySetConnectionParameters(InputArguments args)
        {
            if (args.Contains("server"))
            {
                Config.Instance.Server = args["server"];
            }
            else
            {
                return false;
            }

            if (args.Contains("database"))
            {
                Config.Instance.Database = args["database"];
            }
            else
            {
                return false;
            }

            if (args.Contains("username"))
            {
                Config.Instance.Username = args["username"];
            }
            else
            {
                return false;
            }

            if (args.Contains("password"))
            {
                Config.Instance.Password = args["password"];
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
