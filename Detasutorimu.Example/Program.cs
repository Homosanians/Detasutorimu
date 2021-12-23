using Detasutorimu.Attributes;
using Detasutorimu.Entities;
using System;

namespace Detasutorimu.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Handler hld = new Handler();
            //var argruments = new string[] { "-g", "-s", "Meow", "--pingpong", "hello world!", "-w" };
            var argruments = new string[] { "-z" };

            new ArgumentParser()
                .WithSettings(new ArgumentParserSettings())
                .Register<Handler>(hld)
                .ParseAndExecute(argruments);
            
            Console.WriteLine(hld.gBool);
            Console.WriteLine(hld.sVar);
        }
    }

    public class Handler
    {
        [Argument("p", "Replies with the context", new string[] { "pingpong" })]
        public void Method1(ArgumentContext ctx)
        {
            Console.WriteLine($"Pong! {ctx.Content}");
        }

        [Argument("w", "Writes something", new string[] { "write" })]
        public void Method2()
        {
            Console.WriteLine("Priveeeet");
        }

        [Argument("g", "A boolean variable")]
        public bool gBool;

        [Argument("s", "A string variable")]
        public string sVar;
    }
}
