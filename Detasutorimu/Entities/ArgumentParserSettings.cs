using System;
using System.Collections.Generic;
using System.Text;

namespace Detasutorimu.Entities
{
    public class ArgumentParserSettings
    {
        public string NamePrefix { get; set; } = "-";
        public string AliasPrefix { get; set; } = "--";
        public HelpCommandSettings HelpSettings { get; set; } = new HelpCommandSettings();
    }
}
