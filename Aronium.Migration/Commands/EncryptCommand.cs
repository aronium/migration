using System;

namespace Aronium.Migration.Commands
{
    [Command("encrypt")]
    public class EncryptCommand : CommandBase
    {
        public override void Run(InputArguments args)
        {
            string stringToEncrypt = args["-encrypt"];

            if (string.IsNullOrEmpty(stringToEncrypt))
            {
                Console.Write("Enter value to encrypt: ");

                stringToEncrypt = Console.ReadLine();
            }

            var encryptedValue = stringToEncrypt.Encrypt();
            Console.WriteLine(SEPARATOR_LINES);
            Console.WriteLine(string.Format("Encrypted value: {0}", encryptedValue));
            Console.WriteLine(SEPARATOR_LINES);
        }
    }
}
