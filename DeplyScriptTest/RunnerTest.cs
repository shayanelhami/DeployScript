using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeployScript;

namespace DeplyScriptTest
{
    [TestClass]
    public class RunnerTest
    {
        [TestMethod]
        public void Define_variable_in_script_and_read_its_value()
        {
            var variables = new Variables();
            var runner = new Runner(variables);

            runner.ExecScript("DummyFileName", @"
[define]
some_var=value
");

            Assert.AreEqual("value", variables.Get("some_var"));
        }

        [TestMethod]
        public void Comments_have_no_effect()
        {
            var variables = new Variables();
            var runner = new Runner(variables);

            runner.ExecScript("DummyFileName", @"
[define]
some_var=value1
#some_var=value2

");
            Assert.AreEqual("value1", variables.Get("some_var"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Undefined_variable_should_not_have_value()
        {
            var variables = new Variables();
            var runner = new Runner(variables);

            runner.ExecScript("DummyFileName", @"
[define]
some_var=value
some_var=
");
            variables.Get("some_var");
        }

        [TestMethod]
        public void Variables_from_included_script_should_exist()
        {
            var variables = new Variables();
            var runner = new Runner(variables);

            Prepare.Script("common.deploy", @"
[define]
some_var=value
");

            runner.ExecScript("DummyFileName", @"
include common.deploy
");

            Assert.AreEqual("value", variables.Get("some_var"));
        }

    }
}
