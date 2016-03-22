using System;

namespace Aronium.Migration.Commands
{
    [Command("bootstrap", "Initializes database and configuration")]
    internal class BootstrapCommand : DataCommandBase
    {
        public override void Run(InputArguments args)
        {
            Console.WriteLine(SEPARATOR_LINES);
            Console.WriteLine("Begin bootstrap process. Set login details.");
            Console.WriteLine(SEPARATOR_LINES);
            Console.Write("Server name: ");
            Config.Instance.Server = Console.ReadLine();

            Console.Write("Database name: ");
            Config.Instance.Database = Console.ReadLine();

            Console.Write("Username: ");
            Config.Instance.Username = Console.ReadLine();

            Console.Write("Password: ");
            Config.Instance.Password = ReadConsolePassword(string.Empty);

            Config.Save();

            Console.WriteLine();
            Console.WriteLine(SEPARATOR_LINES);

            if (this.IsConnectionValid())
            {
                Console.WriteLine("Bootstrap completed.");
            }
            
            Console.WriteLine(SEPARATOR_LINES);
        }

        private string ReadConsolePassword(string password)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                }

                return ReadConsolePassword(password);
            }
            if (key.Key == ConsoleKey.Enter)
            {
                return password;
            }
            else
            {
                password += key.KeyChar.ToString();

                return ReadConsolePassword(password);
            }
        }
    }
}
