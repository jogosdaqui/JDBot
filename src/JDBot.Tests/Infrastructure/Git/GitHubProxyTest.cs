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
            Assert.GreaterOrEqual(actual.Major, 1);
            Assert.GreaterOrEqual(actual.Minor, 11);
            Assert.GreaterOrEqual(actual.Patch, 0);
        }
    }
}
