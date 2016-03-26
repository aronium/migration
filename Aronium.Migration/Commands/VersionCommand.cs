using System;

namespace Aronium.Migration.Commands
{
    [Command("version", "Gets current version for specified module.")]
    public class VersionCommand : DataCommandBase
    {
        public override void Run(InputArguments args)
        {
            var module = args["module"];

            var version = GetCurrentVersion(module);

            Console.WriteLine(string.Format("Current version: {0}", version));
            Console.WriteLine(string.Format("Selected module: {0}", module));
        }
    }
}
