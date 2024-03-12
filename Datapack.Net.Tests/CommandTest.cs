using Datapack.Net.Function.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapack.Net.Tests
{
    public class CommandTest
    {
        [Test]
        public void Say()
        {
            var cmd = new SayCommand("test");
            Assert.That(cmd.Build(), Is.EqualTo("say test"));
        }

        [Test]
        public void Function1()
        {
            var func = new MCFunction(new("test", "func"));
            var cmd = new FunctionCommand(func);
            Assert.That(cmd.Build(), Is.EqualTo("function test:func"));
        }

        [Test]
        public void Function2()
        {
            var func = new MCFunction(new("test", "func"));
            func.Add(new SayCommand("test", true));

            var cmd = new FunctionCommand(func, []);
            Assert.That(cmd.Build(), Is.EqualTo("function test:func {}"));
        }

        [Test]
        public void Function3()
        {
            var func = new MCFunction(new("test", "func"));
            func.Add(new SayCommand("test", true));

            var cmd = new FunctionCommand(func, new Position(4, 4, 4));
            Assert.That(cmd.Build(), Is.EqualTo("function test:func with block 4 4 4"));
        }

        [Test]
        public void Function4()
        {
            var func = new MCFunction(new("test", "func"));
            func.Add(new SayCommand("test", true));

            var cmd = new FunctionCommand(func, new NamedTarget("wah"));
            Assert.That(cmd.Build(), Is.EqualTo("function test:func with entity wah"));
        }

        [Test]
        public void Function5()
        {
            var storage = new Storage(new("test:test"));
            var func = new MCFunction(new("test", "func"));
            func.Add(new SayCommand("test", true));

            var cmd = new FunctionCommand(func, storage);
            Assert.That(cmd.Build(), Is.EqualTo("function test:func with storage test:test"));
        }
    }
}
