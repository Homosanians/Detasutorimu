using Detasutorimu.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Detasutorimu.Entities
{
    public class ArgumentModel
    {
        public MemberInfo Member { get; set; }
        public ArgumentAttribute Argument { get; set; }
        public string Content { get; set; }
    }
}
