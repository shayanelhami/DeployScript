using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeployScript;

namespace DeplyScriptTest
{
    [TestClass]
    public class CompilationTest
    {
        [TestMethod]
        public void Change_setting()
        {
            var webConfig = Prepare.WebConfig();
            var runner = new Runner(new Variables());

            runner.ExecScript("DummyFileName", @"
[compilation]
debug = false
");
            var actualValue = XmlUtil.ReadAttribute(webConfig, "/configuration/system.web/compilation", "debug");

            Assert.AreEqual("false", actualValue);
        }
    }
}
