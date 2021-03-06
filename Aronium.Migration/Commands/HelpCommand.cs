﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aronium.Migration.Commands
{
    [Command("help", "Display this help message")]
    public class HelpCommand : CommandBase
    {
        public override void Run(InputArguments args)
        {
            Console.WriteLine();
            Console.WriteLine("Migration file name pattern is: \"<major>_<minor>__<description>.sql\"");
            Console.WriteLine("  <major>         Major version number");
            Console.WriteLine("  <minor>         Minor version number");
            Console.WriteLine("  <description>   Migration script description");
            Console.WriteLine();
            Console.WriteLine("Usage: migration <command> [parameter]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine();
            foreach(var attr in CommandHelper.List())
            {
                Console.WriteLine($"\t-{attr.Name}\n\t    {attr.Description}");
            }
            //Console.WriteLine("  -bootstrap     Initializes database and configuration");
            //Console.WriteLine("  -up            Executes migration scripts");
            //Console.WriteLine("  -status        Display migration status");
            //Console.WriteLine("  -encrypt       Encrypts provided value");
            //Console.WriteLine("  -new           Creates new migration script with optional name provided");
            //Console.WriteLine("  -help          Display this help message");
            Console.WriteLine();
        }
    }
}
