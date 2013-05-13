using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeployScript;

namespace DeplyScriptTest
{
    [TestClass]
    public class WebConfigTest
    {
        [TestMethod]
        public void Change_setting()
        {
            var webConfig = Prepare.WebConfig();
            var runner = new Runner(new Variables());

            runner.ExecScript("DummyFileName", @"
[settings]
Email.Enable.Ssl = False
");
            var actualValue = XmlUtil.ReadSetting(webConfig, "Email.Enable.Ssl");

            Assert.AreEqual("False", actualValue);
        }
    }
}
