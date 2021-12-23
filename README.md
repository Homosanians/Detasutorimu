# Detasutorimu
Program option parser for C#.

## Overview
```cs
class Program
{
    static void Main()
    {
        Handler hld = new Handler();
        var argruments = new string[] { "-g", "--ping", "hello world!", "-w" };
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
    [Argument("p", "Replies with the context", new string[] { "ping" })]
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

### Help
Usage of non-existent parameters will bring up the help menu.

```
Detasutorimu options parser

Name   Aliases      Desciption
p      (pingpong)   - Replies with the context
w      (write)      - Writes something
g                   - A boolean variable
s                   - A string variable
```
