using Detasutorimu.Attributes;
using System;

namespace Detasutorimu.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Handler hld = new Handler();

            new ArgumentParser()
                //.WithSettings(new ArgumentParserSettings())
                .Register<Handler>(hld)
                .Parse(args);

            Console.WriteLine(hld.good);
        }
    }

    public class Handler
    {
        [Argument("a", "aaaa")]
        public void Aaaaa()
        {
            Console.WriteLine("aaaaaaa");
        }

        [Argument("b", "bbbbbbbbb")]
        public void Bbbb()
        {
            Console.WriteLine("bbbbbb");
        }

        [Argument("g", "good")]
        public bool good;
    }
}
