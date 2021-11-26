using Detasutorimu.Attributes;
using Detasutorimu.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Detasutorimu
{
    // todo: check for name or aliases duplicates and forbid them
    public class ArgumentParser
    {
        //private List<Type> handlers = new List<Type>();
        private Dictionary<Type, object> handlers = new Dictionary<Type, object>(); // type, instance
        private List<ArgumentModel> allAttributes;
        private List<ArgumentModel> parsedAttributes;

        public string HelpText { get; private set; } //

        public ArgumentParser Register<T>(object obj)
        {
            handlers.Add(typeof(T), obj);

            return this;
        }

        public ArgumentParser Register<T>()
        {
            handlers.Add(typeof(T), null);

            return this;
        }

        private List<ArgumentModel> GetArgumentModels()
        {
            return allAttributes;
        }

        private void RefreshAttributesAndUpdateFieldAndProperties()
        {
            allAttributes = new List<ArgumentModel>();

            if (handlers.Count == 0)
            {
                throw new Exception("No handlers registered");
            }

            foreach (var item in handlers)
            {
                MemberInfo[] members = item.Key.GetMembers();
                foreach (var member in members)
                {
                    ArgumentAttribute attr = (ArgumentAttribute)Attribute.GetCustomAttribute(member, typeof(ArgumentAttribute));
                    allAttributes.Add(new ArgumentModel()
                    {
                        Argument = attr,
                        MemberType = member.MemberType
                    });

                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            FieldInfo fieldInfo = item.Key.GetField(member.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                            fieldInfo?.SetValue(item.Value, false);
                            break;
                        case MemberTypes.Property:
                            PropertyInfo propertyInfo = item.Key.GetProperty(member.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                            propertyInfo.SetValue(item.Value, false);
                            break;
                    }
                }
            }

            if (allAttributes.Count == 0)
            {
                throw new Exception("No argument attributes were found.");
            }
        }

        private void InvokeAllMethodsOfAttribute(Type attribute)
        {
            var methods = AppDomain.CurrentDomain.GetAssemblies() // Returns all currenlty loaded assemblies
                .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
                .Where(x => x.IsClass) // only yields classes
                .SelectMany(x => x.GetMethods()) // returns all methods defined in those classes
                .Where(x => x.GetCustomAttributes(attribute, false).FirstOrDefault() != null); // returns only methods that have the InvokeAttribute

            foreach (var method in methods) // iterate through all found methods
            {
                foreach (var attr in parsedAttributes.Where(x => x.MemberType == MemberTypes.Method))
                {
                    if (attr.Argument.Name == method.Name)
                    {
                        var obj = Activator.CreateInstance(method.DeclaringType); // Instantiate the class
                        method.Invoke(obj, null); // invoke the method
                    }
                }
            }
        }

        private ArgumentTypes GetTypeOfArg(string arg)
        {
            if (allAttributes.Count != 0) { throw new Exception("no atributes");  }

            foreach(var attr in allAttributes)
            {
                if (arg == attr.Argument.Name) { return ArgumentTypes.Name; }
                foreach (var alias in attr.Argument.Aliases)
                {
                    if (arg == alias) { return ArgumentTypes.Alias; }
                }
            }

            return ArgumentTypes.Content;
        }

        private ArgumentModel GetArgumentModelByRawValue(string arg)
        {
            if (allAttributes.Count != 0) { throw new Exception("no atributes"); }

            foreach (var attr in allAttributes)
            {
                if (arg == attr.Argument.Name) { return attr; }
                foreach (var alias in attr.Argument.Aliases)
                {
                    if (arg == alias) { return attr; }
                }
            }

            return null;
        }

        public ArgumentParser Parse(string[] args)
        {
            if (args.Length > 0)
            {

                for (int i = 0; i < args.Length; i++)
                {
                    var item = args[i];

                    var model = GetArgumentModelByRawValue(item);
                    if (model != null && i + 1 <= args.Length)
                    {
                        if (GetTypeOfArg(args[i + 1]) == ArgumentTypes.Content)
                        {
                            model.Content = args[i + 1];
                            parsedAttributes.Add(model);
                        }
                    }
                }

                RefreshAttributesAndUpdateFieldAndProperties();

                InvokeAllMethodsOfAttribute(typeof(ArgumentAttribute));

                foreach (var attr in allAttributes)
                {

                }
            }

            return this;
        }
    }

    enum ArgumentTypes
    {
        Name,
        Alias,
        Content
    }
}
