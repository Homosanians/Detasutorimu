using Detasutorimu.Attributes;
using Detasutorimu.Entities;
using NUnit.Framework;
using System;

namespace Detasutorimu.Tests
{
    public class ClientCodeBlunderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DuplicatedAttributeCauseExceptionWithinMethodsTest()
        {
            ClientCodeBlunderTestsHandler hld = new ClientCodeBlunderTestsHandler();
            var argruments = new string[] { "-p" };

            try
            {
                new ArgumentParser()
                    .WithSettings(new ArgumentParserSettings())
                    .Register<ClientCodeBlunderTestsHandler>(hld)
                    .ParseAndExecute(argruments);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception:\n{e.Message} {e.StackTrace} {e.Source}");
                Assert.Pass();
            }
        }
    }

    public class ClientCodeBlunderTestsHandler
    {
        [Argument("p", "A method 1", new string[] { "pingpong" })]
        public void Method1()
        {
            Assert.Fail();
        }

        [Argument("p", "A method 2", new string[] { "pingpong" })]
        public void Method2()
        {
            Assert.Fail();
        }
    }
}