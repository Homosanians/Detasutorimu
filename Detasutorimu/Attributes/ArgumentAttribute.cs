using System;
using System.Collections.Generic;
using System.Text;

namespace Detasutorimu.Attributes
{
    /// <summary>
    /// Marks the execution information for a parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ArgumentAttribute : Attribute
    {
        public string Name { get; }
        public string Desciption { get; }
        public bool Required { get; }
        public string[] Aliases { get; }

        public ArgumentAttribute(string name, string description, bool required = false)
        {
            this.Name = name;
            this.Desciption = description;
            this.Required = required;
        }

        public ArgumentAttribute(string name, string description, string[] aliases, bool required = false)
        {
            this.Name = name;
            this.Desciption = description;
            this.Aliases = aliases;
            this.Required = required;
        }
    }
}
