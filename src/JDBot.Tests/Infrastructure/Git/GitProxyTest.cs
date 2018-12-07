using System;
using System.IO;
using JDBot.Infrastructure.Git;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Git
{
    [TestFixture]
    public class GitProxyTest
    {
        [Test]
        public void Commands_TempFolder_Success()
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            Environment.CurrentDirectory = tempFolder;

            var target = new GitProxy();
            target.Init();

            File.WriteAllText("dummy1.txt", "dummy");
            target.Add("*");
            target.Commit("message1");

            target.CheckoutNewBranch("test");
            File.WriteAllText("dummy2.txt", "dummy");
            target.Add("*");
            target.Commit("message2");

            target.Checkout("master");

            var actualEx = Assert.Catch<InvalidOperationException>(target.Push);
            StringAssert.Contains("No configured push destination.", actualEx.Message);

            actualEx = Assert.Catch<InvalidOperationException>(target.PushTags);
            StringAssert.Contains("No configured push destination.", actualEx.Message);
        }
    }
}
