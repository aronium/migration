using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;

namespace Aronium.Migration.Commands
{
    public abstract class CommandBase
    {
        private string _rootPath;
        private string _scriptsDirectoryPath;

        public const string SEPARATOR_LINES = "------------------------------------------------------------------------";

        protected string ScriptsDirectoryPath
        {
            get
            {
                if (_scriptsDirectoryPath == null)
                    _scriptsDirectoryPath = Path.Combine(this.RootPath, "Migrations", "Scripts");
                return _scriptsDirectoryPath;
            }
        }

        protected string RootPath
        {
            get
            {
                if (_rootPath == null)
                    _rootPath = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;

                return _rootPath;
            }
        }

        public abstract void Run(InputArguments args);

    }
}
