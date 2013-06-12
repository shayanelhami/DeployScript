using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeployScript;

namespace DeplyScriptTest
{
    [TestClass]
    public class SmtpTest
    {
        // some values belong to smtp node itself and some to network node under it
        // we test them separately 

        [TestMethod]
        public void Change_SMTP_node_settings()
        {
            var webConfig = Prepare.WebConfig();
            var runner = new Runner(new Variables());

            runner.ExecScript("DummyFileName", @"
[smtp]
from = a@b.com
");
            var actualValue = XmlUtil.ReadSmtpField(webConfig, fieldName: "from");

            Assert.AreEqual("a@b.com", actualValue);
        }

        [TestMethod]
        public void Change_SMTP_network_settings()
        {
            var webConfig = Prepare.WebConfig();
            var runner = new Runner(new Variables());

            runner.ExecScript("DummyFileName", @"
[smtp]
host=host1
port=111
userName=userName1
password=password1
defaultCredentials=false
");
            var actualHost = XmlUtil.ReadSmtpNetworkField(webConfig, "host");
            var actualPort = XmlUtil.ReadSmtpNetworkField(webConfig, "port");
            var actualUserName = XmlUtil.ReadSmtpNetworkField(webConfig, "userName");
            var actualPassword = XmlUtil.ReadSmtpNetworkField(webConfig, "password");
            var actualDefaultCredentials = XmlUtil.ReadSmtpNetworkField(webConfig, "defaultCredentials");

            Assert.AreEqual("host1", actualHost);
            Assert.AreEqual("111", actualPort);
            Assert.AreEqual("userName1", actualUserName);
            Assert.AreEqual("password1", actualPassword);
            Assert.AreEqual("false", actualDefaultCredentials);
        }

        [TestMethod]
        public void SMTP_attribute_must_be_created_if_not_exists()
        {
            var webConfig = Prepare.WebConfig();
            var runner = new Runner(new Variables());

            runner.ExecScript("DummyFileName", @"
[smtp]
LockItem=false
EnableSsl=true
");
            var actualLockItem = XmlUtil.ReadSmtpField(webConfig, "LockItem");
            var actualEnableSsl = XmlUtil.ReadSmtpNetworkField(webConfig, "EnableSsl");

            Assert.AreEqual("false", actualLockItem);
            Assert.AreEqual("true", actualEnableSsl);
        }

    }
}
