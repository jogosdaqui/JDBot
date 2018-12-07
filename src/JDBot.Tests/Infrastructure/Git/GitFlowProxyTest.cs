using System;
using System.IO;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.Git;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Git
{
    [TestFixture]
    public class GitFlowProxyTest
    {
        [Test]
        public void Commands_TempFolder_Success()
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            Environment.CurrentDirectory = tempFolder;

            var git = new GitProxy();
            git.Init();

            var target = new GitFlowProxy();
            target.Init();

            File.WriteAllText("dummy1.txt", "dummy");
            git.Add("*");
            git.Commit("message1");

            var version = new SemanticVersioning(1, 2, 3);
            target.StartRelease(version);
            target.FinishRelease(version, "release message");
        }
    }
}
