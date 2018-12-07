﻿using System.Threading.Tasks;
using JDBot.Domain.Sites;
using JDBot.Infrastructure.Framework;
using NSubstitute;
using NUnit.Framework;

namespace JDBot.Tests.Domain.Sites
{
    [TestFixture]
    public class SiteReleaserTest
    {
        [Test]
        public async Task ReleaseAsync_NoArgs_ReleaseNewVersion()
        {
            var fs = Substitute.For<IFileSystem>();
            var gitHub = Substitute.For<IGitHubProxy>();
            gitHub.GetLatestReleaseAsync().Returns((arg) => new SemanticVersioning(1, 9, 0));
           
            var git = Substitute.For<IGitProxy>();
            var gitFlow = Substitute.For<IGitFlowProxy>();

            var target = new SiteReleaser("folder", fs, gitHub, git, gitFlow);
            var actual = await target.ReleaseAsync("release message");

            Assert.AreEqual(1, actual.Major);
            Assert.AreEqual(10, actual.Minor);
            Assert.AreEqual(0, actual.Patch);

            fs.Received().ChangeCurrentDirectory("folder");
            gitFlow.Received().StartRelease(actual);
            gitFlow.Received().FinishRelease(actual, "release message");
            git.Received().Checkout("master");
            git.Received().Push();
            git.Received().PushTags();
            git.Received().Checkout("develop");
            git.Received().Push();
        }
    }
}