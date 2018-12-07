using System;
using System.Threading.Tasks;
using JDBot.Infrastructure.Git;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Git
{
    [TestFixture]
    public class GitHubProxyTest
    {
        [Test]
        public async Task GetLatestReleaseAsync_NoArgs_LatestRelease()
        {
            var target = new GitHubProxy();
            var actual = await target.GetLatestReleaseAsync();
            Assert.AreEqual("1.9.0", actual.ToString());
        }
    }
}
