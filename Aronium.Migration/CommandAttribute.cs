using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aronium.Migration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
