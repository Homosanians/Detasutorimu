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
                    if (attr.Argument.Name == method.GetCustomAttribute<ArgumentAttribute>().Name)
                    {
                        var obj = Activator.CreateInstance(method.DeclaringType);
                        try
                        {
                            if (method.GetParameters().Length == 0)
                            {
                                method.Invoke(obj, null);
                            }
                            else if (method.GetParameters().Length == 1)
                            {
                                ArgumentContext ctx = new ArgumentContext();
                                if (!string.IsNullOrEmpty(attr.Content))
                                {
                                    ctx.ContextPresent = true;
                                    ctx.Content = attr.Content;
                                }
                                method.Invoke(obj, new object[] { ctx });
                            }
                        }
                        // Количество аргументов не совпадает с переданным или задан необрабатываемый аргумент.
                        catch (TargetParameterCountException)
                        {
                            throw new Exception("Method parameters are not expected. It can only be ArgumentContext or nothing.");
                        }
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
                if (i + 1 < args.Length)
                {
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

                //Console.WriteLine($"UTILS:GetParsedAttributes >> PREV {prevItem} {prevType} | CURR {item} {currentType} | NEXT {nextItem} {nextType}");

                if (currentType == ArgumentTypes.Name || currentType == ArgumentTypes.Alias)
                {
                    foreach (var attr in allAttributes)
                    {
                        if (item == attr.Argument.Name)
                        {
                            if (nextType == ArgumentTypes.Content)
                            {
                                attr.Content = nextItem;
                            }
                            parsedAttributes.Add(attr);
                        }
                        if (attr.Argument.Aliases != null)
                        {
                            foreach (var alias in attr.Argument.Aliases)
                            {
                                if (item == alias)
                                {
                                    if (nextType == ArgumentTypes.Content)
                                    {
                                        attr.Content = nextItem;
                                    }
                                    parsedAttributes.Add(attr);
                                }
                            }
                        }
                    }
                }
            }

            return parsedAttributes;
        }

        // TODO delegates container to parse other Types besides string and bool
        private static object GetParsedContent(Type type, bool contentPresent, string content)
        {
            if (type == typeof(bool))
            {
                if (contentPresent)
                {
                    try
                    {
                        return bool.Parse(content);
                    }
                    catch (Exception) { }
                }
                return true;
            }
            else if (type == typeof(string))
            {
                return content;
            }

            throw new Exception("Type is not supported yet.");
        }

        internal static void SetValuesForAttributes(Dictionary<Type, object> container, List<ArgumentModel> argumentOfVariablesToBeSet)
        {
            foreach (var item in container)
            {
                MemberInfo[] members = item.Key.GetMembers();
                foreach (var member in members)
                {
                    ArgumentAttribute attr = (ArgumentAttribute)Attribute.GetCustomAttribute(member, typeof(ArgumentAttribute));
                    if (attr != null && argumentOfVariablesToBeSet.Any(x => x.Argument.Name == attr.Name && x.Argument.Aliases == attr.Aliases && x.Argument.Desciption == attr.Desciption))
                    {
                        switch (member.MemberType)
                        {
                            case MemberTypes.Field:
                                FieldInfo fieldInfo = item.Key.GetField(member.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                                string fieldContent = argumentOfVariablesToBeSet.Find(x => x.Argument.Name == attr.Name && x.Argument.Aliases == attr.Aliases && x.Argument.Desciption == attr.Desciption).Content;
                                fieldInfo.SetValue(item.Value, GetParsedContent(fieldInfo.FieldType, !string.IsNullOrEmpty(fieldContent), fieldContent));
                                break;
                            case MemberTypes.Property:
                                PropertyInfo propertyInfo = item.Key.GetProperty(member.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                                string propertyContent = argumentOfVariablesToBeSet.Find(x => x.Argument.Name == attr.Name && x.Argument.Aliases == attr.Aliases && x.Argument.Desciption == attr.Desciption).Content;
                                propertyInfo.SetValue(item.Value, GetParsedContent(propertyInfo.PropertyType, !string.IsNullOrEmpty(propertyContent), propertyContent));
                                break;
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
