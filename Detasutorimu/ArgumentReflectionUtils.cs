using Detasutorimu.Attributes;
using Detasutorimu.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Detasutorimu
{
    internal static class ArgumentReflectionUtils
    {
        internal static void InvokeAllMethodsOfAttribute(Type attribute, List<ArgumentModel> argumentOfMethodsToBeInvoked)
        {
            var methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes()) // returns all types defined in these assemblies
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttributes(attribute, false).FirstOrDefault() != null); // returns only methods that have the attribute

            foreach (var method in methods)
            {
                foreach (var attr in argumentOfMethodsToBeInvoked.Where(x => x.Member.MemberType == MemberTypes.Method))
                {
                    if (attr.Argument.Name == method.Name)
                    {
                        var obj = Activator.CreateInstance(method.DeclaringType);
                        method.Invoke(obj, null);
                    }
                }
            }
        }

        internal static List<ArgumentModel> GetAllAttributes(Dictionary<Type, object> container)
        {
            if (container.Count == 0)
            {
                throw new Exception("No handlers registered");
            }

            List<ArgumentModel> allAttributes = new List<ArgumentModel>();

            foreach (var item in container)
            {
                MemberInfo[] members = item.Key.GetMembers();
                foreach (var member in members)
                {
                    ArgumentAttribute attr = (ArgumentAttribute)Attribute.GetCustomAttribute(member, typeof(ArgumentAttribute));
                    if (attr != null)
                    {
                        allAttributes.Add(new ArgumentModel()
                        {
                            Argument = attr,
                            Member = member
                        });
                    }
                }
            }

            if (allAttributes.Count == 0)
            {
                throw new Exception("No argument attributes were found.");
            }

            return allAttributes;
        }

        internal static List<ArgumentModel> GetParsedAttributes(Dictionary<Type, object> container, List<ArgumentModel> allAttributes, string[] args, ArgumentParserSettings settings)
        {
            List<ArgumentModel> parsedAttributes = new List<ArgumentModel>();

            for (int i = 0; i < args.Length; i++)
            {
                string item = RemovePrefixes(args[i], new string[] { settings.NamePrefix, settings.AliasPrefix });
                ArgumentTypes currentType = GetTypeOfArg(allAttributes, args[i], settings);

                string nextItem = null;
                ArgumentTypes nextType = ArgumentTypes.None;
                if (i + 1 < args.Length) {
                    nextItem = RemovePrefixes(args[i + 1], new string[] { settings.NamePrefix, settings.AliasPrefix });
                    nextType = GetTypeOfArg(allAttributes, args[i + 1], settings);
                }

                string prevItem = null;
                ArgumentTypes prevType = ArgumentTypes.None;
                if (i - 1 >= 0)
                {
                    prevItem = RemovePrefixes(args[i - 1], new string[] { settings.NamePrefix, settings.AliasPrefix });
                    prevType = GetTypeOfArg(allAttributes, args[i - 1], settings);
                }

                Console.WriteLine($"UTILS:92 >> PREV {prevItem} | CURR {item} | NEXT {nextItem}");

                if (currentType == ArgumentTypes.Name || currentType == ArgumentTypes.Alias)
                {
                    foreach (var attr in allAttributes)
                    {
                        if (nextType == ArgumentTypes.Content)
                        {
                            attr.Content = nextItem;
                        }

                        if (item == attr.Argument.Name) { parsedAttributes.Add(attr); }
                        if (attr.Argument.Aliases != null)
                        {
                            foreach (var alias in attr.Argument.Aliases)
                            {
                                if (item == alias) { parsedAttributes.Add(attr); }
                            }
                        }
                    }
                }
            }

            return parsedAttributes;
        }

        internal static void SetValuesForAttributes(Dictionary<Type, object> container, List<ArgumentModel> arguments)
        {
            //not sure it works
            foreach (var item in container)
            {
                MemberInfo[] members = item.Key.GetMembers();
                foreach (var member in members)
                {
                    ArgumentAttribute attr = (ArgumentAttribute)Attribute.GetCustomAttribute(member, typeof(ArgumentAttribute));
                    if (attr != null)
                    {
                        switch (item.Key.MemberType)
                        {
                            case MemberTypes.Field:
                                FieldInfo fieldInfo = item.Key.GetField(member.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                                fieldInfo?.SetValue(item.Value, false);
                                break;
                            case MemberTypes.Property:
                                PropertyInfo propertyInfo = item.Key.GetProperty(member.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                                propertyInfo?.SetValue(item.Value, false);
                                break;
                            default:
                                throw new Exception("Not possible");
                        }
                    }
                }
            }
        }

        private static string RemovePrefix(string value, string prefix)
        {
            return value.TrimStart(prefix.ToCharArray());
        }

        private static string RemovePrefixes(string value, string[] prefixes)
        {
            foreach (var prefix in prefixes)
            {
                value = value.TrimStart(prefix.ToCharArray());
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allAttributes"></param>
        /// <param name="arg">Raw argument with prefix</param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static ArgumentTypes GetTypeOfArg(List<ArgumentModel> allAttributes, string arg, ArgumentParserSettings settings)
        {
            if (allAttributes.Count == 0) { throw new Exception("no atributes"); }

            arg = RemovePrefixes(arg, new string[] { settings.NamePrefix, settings.AliasPrefix });

            foreach (var attr in allAttributes)
            {
                if (arg == attr.Argument.Name) { return ArgumentTypes.Name; }
                if (attr.Argument.Aliases != null)
                {
                    foreach (var alias in attr.Argument.Aliases)
                    {
                        if (arg == alias) { return ArgumentTypes.Alias; }
                    }
                }
            }

            return ArgumentTypes.Content;
        }
    }
}
