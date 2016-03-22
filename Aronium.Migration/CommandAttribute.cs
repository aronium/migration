using System;

namespace Aronium.Migration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name)
        {
            this.Name = name;
        }

        public CommandAttribute(string name, string description) : this(name)
        {
            this.Description = description;
        }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
