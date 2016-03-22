using Aronium.Migration.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aronium.Migration
{
    public static class CommandHelper
    {
        public static IEnumerable<CommandAttribute> List(string ns = "Aronium.Migration.Commands")
        {
            // Find all classes in specified namespace
            var attrs = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsClass
                        && t.Namespace == ns
                        && !t.IsAbstract
                        && typeof(CommandBase).IsAssignableFrom(t)
                        && t.GetCustomAttributes(typeof(CommandAttribute), false).Any()
                        select t.GetCustomAttributes(typeof(CommandAttribute), true).First();

            return attrs.Cast<CommandAttribute>();
        }
    }
}
