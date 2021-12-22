using Detasutorimu.Attributes;
using Detasutorimu.Entities;
using NUnit.Framework;

namespace Detasutorimu.Tests
{
    public class InvokeTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InvokeMethodByNameTest()
        {
            InvokeTestsHandler hld = new InvokeTestsHandler();
            var argruments = new string[] { "-p" };

            new ArgumentParser()
                .WithSettings(new ArgumentParserSettings())
                .Register<InvokeTestsHandler>(hld)
                .ParseAndExecute(argruments);
        }

        [Test]
        public void InvokeMethodByAliasTest()
        {
            InvokeTestsHandler hld = new InvokeTestsHandler();
            var argruments = new string[] { "--pingpong" };

            new ArgumentParser()
                .WithSettings(new ArgumentParserSettings())
                .Register<InvokeTestsHandler>(hld)
                .ParseAndExecute(argruments);
        }

        [Test]
        public void SetBoolByNameTest()
        {
            InvokeTestsHandler hld = new InvokeTestsHandler();
            var argruments = new string[] { "-g" };

            new ArgumentParser()
                .WithSettings(new ArgumentParserSettings())
                .Register<InvokeTestsHandler>(hld)
                .ParseAndExecute(argruments);

            Assert.IsTrue(hld.gBool);
        }

        [Test]
        public void SetBoolByAliasTest()
        {
            InvokeTestsHandler hld = new InvokeTestsHandler();
            var argruments = new string[] { "--gVariable" };

            new ArgumentParser()
                .WithSettings(new ArgumentParserSettings())
                .Register<InvokeTestsHandler>(hld)
                .ParseAndExecute(argruments);

            Assert.IsTrue(hld.gBool);
        }
    }

    public class InvokeTestsHandler
    {
        [Argument("p", "A method", new string[] { "pingpong" })]
        public void Method1(ArgumentContext ctx)
        {
            Assert.Pass();
        }

        [Argument("g", "A variable", new string[] { "gVariable" })]
        public bool gBool;
    }
}