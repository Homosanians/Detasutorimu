using Detasutorimu.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Detasutorimu.Attributes
{
    // TODO Option arg | executable arg
    /// <summary>
    /// Marks the execution information for a parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ArgumentAttribute : Attribute
    {
        //TODO privide ctx (context) whenever implementation is aclled
        public string Name { get; }
        public string Desciption { get; }
        public bool Required { get; }
        public string[] Aliases { get; }
        public ExpectedTypes ExpectedType { get; }

        public ArgumentAttribute(string name, string description, ExpectedTypes expectedType = ExpectedTypes.None, bool required = false)
        {
            this.Name = name;
            this.Desciption = description;
            this.Required = required;
            this.ExpectedType = expectedType;
        }

        public ArgumentAttribute(string name, string description, string[] aliases, ExpectedTypes expectedType = ExpectedTypes.None, bool required = false)
        {
            this.Name = name;
            this.Desciption = description;
            this.Aliases = aliases;
            this.Required = required;
            this.ExpectedType = expectedType;
        }
    }
}
