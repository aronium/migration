using System;

namespace Aronium.Migration.Commands
{
    [Command("bootstrap")]
    internal class BootstrapCommand : DataCommandBase
    {
        public override void Run(InputArguments args)
        {
            Console.WriteLine(SEPARATOR_LINES);
            Console.WriteLine("Begin bootstrap process.");

            Console.Write("Database file: ");
            Config.Instance.Database = Console.ReadLine();
            
            Config.Save();

            Console.WriteLine();
            Console.WriteLine(SEPARATOR_LINES);

            if (this.IsConnectionValid())
            {
                Console.WriteLine("Bootstrap completed.");
            }
            
            Console.WriteLine(SEPARATOR_LINES);
        }
    }
}
