using Aronium.Migration.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aronium.Migration
{
    public static class CommandHelper
    {
        /// <summary>
        /// List all available commands in specified namespace.
        /// </summary>
        /// <param name="ns">Namespace.</param>
        /// <returns>List of available commands as command attributes.</returns>
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

        /// <summary>
        /// Gets version string.
        /// </summary>
        /// <param name="value">Version value.</param>
        /// <returns>Version string.</returns>
        public static string ToVersionString(this decimal value)
        {
            return value.ToString("0.0#####", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets version from version string.
        /// </summary>
        /// <param name="value">Version.</param>
        /// <returns>Version value.</returns>
        public static decimal ToVersion(this string value)
        {
            return Convert.ToDecimal(value, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
