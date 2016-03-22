using Aronium.Migration.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aronium.Migration.Commands
{
    public abstract class CommandBase
    {
        private string _rootPath;
        private string _scriptsDirectoryPath;

        public const string SEPARATOR_LINES = "------------------------------------------------------------------------";

        /// <summary>
        /// Gets script directory path.
        /// </summary>
        protected string ScriptsDirectoryPath
        {
            get
            {
                if (_scriptsDirectoryPath == null)
                    _scriptsDirectoryPath = Path.Combine(this.RootPath, "Migrations", "Scripts");
                return _scriptsDirectoryPath;
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
        protected IEnumerable<string> GetFiles()
        {
            return Directory.GetFiles(ScriptsDirectoryPath, "*.sql").OrderBy(x => x);
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

            MigrationStatus status = new MigrationStatus()
            {
                Version = GetFileVersion(file).ToVersionString(),
                FileName = info.Name,
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
