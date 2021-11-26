using Detasutorimu.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Detasutorimu.Entities
{
    public class ArgumentModel
    {
        public MemberTypes MemberType { get; set; }
        public ArgumentAttribute Argument { get; set; }
        public object Content { get; set; }
    }
}
