# Detasutorimu
Program option parser for C#

## Overview
```cs
class Program
{
    static void Main()
    {
        Handler hld = new Handler();
        var argruments = new string[] { "-g", "--pingpong", "hello world!", "-w" };
        new ArgumentParser()
            .WithSettings(new ArgumentParserSettings())
            .Register<Handler>(hld)
            .ParseAndExecute(argruments);

        Console.WriteLine(hld.gBool);
        
        // OUTPUT
        // Pong! hello world!
        // Priveeeet
        // True
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

    [Argument("g", "A variable")]
    public bool gBool;
}
```
