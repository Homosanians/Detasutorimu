using System;
using System.Collections.Generic;
using System.Text;

namespace Detasutorimu
{
    public abstract class HandlerBase
    {
        internal HandlerBase(ArgumentContext context)
        {
            Context = context;
        }

        protected ArgumentContext Context { get; private set; }

        internal T Make<T>(params object[] args) where T : HandlerBase
        {
            object[] arrayWithTheContext = new[] { this.Context };

            object[] combined = new object[args.Length + arrayWithTheContext.Length];
            Array.Copy(arrayWithTheContext, combined, arrayWithTheContext.Length);
            Array.Copy(args, 0, combined, arrayWithTheContext.Length, args.Length);

            T instance = (T)Activator.CreateInstance(typeof(T), combined);
            return instance;
        }
    }
}
