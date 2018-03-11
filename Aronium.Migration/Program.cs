using Aronium.Migration.Commands;
using System;
using System.Linq;
using System.Reflection;

namespace Aronium.Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No command specified. Please provide command to execute.");
            }
            else
            {
                var arguments = new InputArguments(args);

                // Declare namespace to search types in
                string @namespace = "Aronium.Migration.Commands";

                // Get first argument to match CommandAttribute.Name property of decorated type.
                var firstArg = arguments.GetPeeledKey(args[0]);

                // Declare command base to use.
                CommandBase commandBase = null;

                // Find all classes in specified namespace
                var commands = from t in Assembly.GetExecutingAssembly().GetTypes() 
                               where t.IsClass && t.Namespace == @namespace && !t.IsAbstract && typeof(CommandBase).IsAssignableFrom(t) &&
                               t.GetCustomAttributes(typeof(CommandAttribute), false).Any(x => ((CommandAttribute)x).Name.Equals(firstArg, StringComparison.OrdinalIgnoreCase))
                               select t;

                if (commands.Any())
                {
                    commandBase = Activator.CreateInstance(commands.First()) as CommandBase;

                    if (commandBase != null)
                    {
                        if (commandBase is DataCommandBase)
                        {
                            DataCommandBase.TrySetConnectionParameters(arguments);
                        }

                        commandBase.Run(arguments);
                    }
                }
                else
                {
                    Console.WriteLine("COMMAND NOT RECOGNIZED!\nPLEASE PROVIDE COMMAND TO EXECUTE.");
                }
            }
        }
    }
}
