using Aronium.Migration.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aronium.Migration.Commands
{
    public abstract class CommandBase
    {
        private string _rootPath;
        private string _scriptsDirectoryPath;

        internal const string SEPARATOR_LINES = "------------------------------------------------------------------------";

        /// <summary>
        /// Gets or sets migration script directory path.
        /// <para>If value is not set, default directory will be used.</para>
        /// </summary>
        protected string MigrationsDirectory
        {
            get
            {
                if (_scriptsDirectoryPath == null)
                    _scriptsDirectoryPath = Path.Combine(this.RootPath, "Migrations", "Scripts");
                return _scriptsDirectoryPath;
            }
            set
            {
                _scriptsDirectoryPath = value;
            }
        }

        /// <summary>
        /// Gets root path.
        /// </summary>
        protected string RootPath
        {
            get
            {
                if (_rootPath == null)
                    _rootPath = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;

                return _rootPath;
            }
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

            return decimal.Parse(extractedVersion, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets all migration scripts from script directory.
        /// </summary>
        /// <returns>List of files.</returns>
        protected IEnumerable<MigrationStatus> GetFiles()
        {
            foreach(var file in Directory.GetFiles(MigrationsDirectory, "*.sql", SearchOption.AllDirectories).OrderBy(x => x))
            {
                yield return ParseFileName(file);
            }
        }

        /// <summary>
        /// Parse file name to migration status.
        /// </summary>
        /// <param name="file">File name to parse.</param>
        /// <returns>MigrationStatus instance.</returns>
        protected MigrationStatus ParseFileName(string file)
        {
            FileInfo info = new FileInfo(file);

            var name = info.Name;
            var description = name.Replace(info.Extension, string.Empty).Substring(name.IndexOf("__") + 2).Replace("_", " ");

            var pathWithoutRootDirectory = file.Replace(MigrationsDirectory, "");
            string[] dirs = pathWithoutRootDirectory.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

            // Check for mudule as a sub directory of main scripts directory
            string module = null;

            // If subfolder exists, using directory name as a module
            // Split characters contains at least 1 item as file name. if directory exists, dirs array will contain 2 items in array, directory and file name.
            if (dirs.Length > 1)
            {
                // Take first directory level as a module name, no need to go deeper in subdirectories, modules should have unique names
                module = dirs[0];
            }

            MigrationStatus status = new MigrationStatus()
            {
                Version = GetFileVersion(file).ToVersionString(),
                FileName = name,
                Path = file,
                Module = module,
                Description = description
            };

            return status;
        }

        /// <summary>
        /// Runs command.
        /// </summary>
        /// <param name="args">Input arguments.</param>
        public abstract void Run(InputArguments args);
    }
}
