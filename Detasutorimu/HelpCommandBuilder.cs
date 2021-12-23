using Detasutorimu.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Detasutorimu
{
    public class HelpCommandBuilder
    {
        private HelpCommandSettings settings;
        private bool built = false;
        private List<ArgumentModel> argumentModels = new List<ArgumentModel>();
        private string builtText;

        public HelpCommandBuilder(HelpCommandSettings settings)
        {
            this.settings = settings;
        }

        public HelpCommandBuilder AddArgument(ArgumentModel argument)
        {
            argumentModels.Add(argument);

            return this;
        }

        public HelpCommandBuilder AddArguments(ArgumentModel[] arguments)
        {
            argumentModels.AddRange(arguments);

            return this;
        }

        public HelpCommandBuilder AddArguments(ICollection<ArgumentModel> arguments)
        {
            argumentModels.AddRange(arguments);

            return this;
        }

        private int ClampBelow(int value, int x)
        {
            if (x < value)
                return value;
            return x;
        }

        // TODO
        public string Build()
        {
            if (!built)
            {
                int nameMaxChars = 7;
                int aliasesMaxChars = 13;
                int descriptionMaxChars = 0;

                foreach (var item in argumentModels)
                {
                    string aliases = "";
                    if (item.Argument.Aliases != null)
                    {
                        aliases = String.Join(", ", item.Argument.Aliases);
                    }

                    if (item.Argument.Name.Length > nameMaxChars)
                        nameMaxChars = item.Argument.Name.Length;
                    if (aliases.Length > aliasesMaxChars)
                        aliasesMaxChars = aliases.Length;
                    if (item.Argument.Desciption.Length > descriptionMaxChars)
                        descriptionMaxChars = item.Argument.Desciption.Length;
                }

                string helpText = $"Name{new String(' ', ClampBelow(1, nameMaxChars-4))}Aliases{new String(' ', ClampBelow(1, aliasesMaxChars - 7))}Desciption\n";

                foreach (var item in argumentModels)
                {
                    string aliases = "";
                    if (item.Argument.Aliases != null)
                    {
                        aliases = String.Join(", ", item.Argument.Aliases);

                        helpText +=
                            $"{item.Argument.Name}{new String(' ', ClampBelow(1, nameMaxChars - item.Argument.Name.Length))}({aliases}){new String(' ', ClampBelow(1, aliasesMaxChars - aliases.Length - 2))}— {item.Argument.Desciption}\n";
                    }
                    else
                    {
                        helpText +=
                            $"{item.Argument.Name}{new String(' ', ClampBelow(1, nameMaxChars - item.Argument.Name.Length))}{new String(' ', ClampBelow(1, aliasesMaxChars - aliases.Length))}— {item.Argument.Desciption}\n";
                    }
                }

                helpText = String.Format(settings.HelpMessageFormat, helpText);

                if (settings.PrintToConsoleByDefault)
                {
                    Console.WriteLine(helpText);
                }

                built = true;
                builtText = helpText;
            }

            return builtText;
        }
    }
}
