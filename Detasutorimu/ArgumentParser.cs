using Detasutorimu.Attributes;
using Detasutorimu.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Detasutorimu
{
    // TODO check for name or aliases duplicates. warn about it and use latest or valid
    public class ArgumentParser
    {
        private ArgumentParserSettings argumentParserSettings = new ArgumentParserSettings();

        private Dictionary<Type, object> container = new Dictionary<Type, object>(); // (Type, Instance) of handlers
        private List<ArgumentModel> allAttributes;
        private List<ArgumentModel> parsedAttributes;

        // TODO register assembly(ies)
        // register<>(inst) should handle isntanec and it values i nit
        public ArgumentParser Register<T>(object obj)
        {
            container.Add(typeof(T), obj);

            return this;
        }

        public ArgumentParser Register<T>()
        {
            container.Add(typeof(T), null);

            return this;
        }

        private List<ArgumentModel> GetArgumentModels()
        {
            return allAttributes;
        }

        public ArgumentParser ParseAndExecute(string[] args)
        {
            if (args.Length > 0)
            {
                allAttributes = ArgumentReflectionUtils.GetAllAttributes(container);
                parsedAttributes = ArgumentReflectionUtils.GetParsedAttributes(container, allAttributes, args, argumentParserSettings);
                ArgumentReflectionUtils.InvokeAllMethodsOfAttribute(typeof(ArgumentAttribute), parsedAttributes);
                ArgumentReflectionUtils.SetValuesForAttributes(container, parsedAttributes);
            }

            return this;
        }

        public ArgumentParser WithSettings(ArgumentParserSettings argumentParserSettings)
        {
            this.argumentParserSettings = argumentParserSettings;

            return this;
        }
    }
}
