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
                .Register<Handler>(hld)
                .Parse(args);

            Console.WriteLine(hld.good);
        }
    }

    public class Handler
    {
        [Argument("a", "aaaa")]
        public void Bum()
        {
            Console.WriteLine("aaaaaaa");
        }

        [Argument("b", "bbbbbbbbb")]
        public void Shit()
        {
            Console.WriteLine("bbbbbb");
        }

        [Argument("g", "good")]
        public bool good;
    }
}
