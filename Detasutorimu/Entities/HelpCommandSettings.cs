using System;
using System.Collections.Generic;
using System.Text;

namespace Detasutorimu
{
    public class HelpCommandSettings
    {
        /// <summary>
        /// "Detasutorimu options parser\n\n{0}\n" by default. POSIX sensitive.
        /// </summary>
        public string HelpMessageFormat = "Detasutorimu options parser\n\n{0}\n";

        public bool UseHelp { get; set; } = true;

        public bool PrintToConsoleByDefault { get; set; } = true;
    }
}
