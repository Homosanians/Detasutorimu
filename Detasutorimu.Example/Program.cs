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
            var argruments = new string[] { "-g", "--bbbbbbbbb" };
            new ArgumentParser()
                .WithSettings(new ArgumentParserSettings())
                .Register<Handler>(hld)
                .Parse(argruments);

            Console.WriteLine(hld.good);
        }
    }

    public class Handler
    {
        [Argument("a", "Aaaaa", new string[] { "aaaa" })]
        public void Aaaaa(ArgumentContext ctx)
        {
            Console.WriteLine($"aaaaaaa {ctx.ToString()}");
        }

        [Argument("b", "Bebra", new string[] { "bbbbbbbbb" })]
        public void Bbbb()
        {
            Console.WriteLine("bbbbbb");
        }

        [Argument("g", "good")]
        public bool good;
    }
}
