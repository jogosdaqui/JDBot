using System.Linq;
using System.Threading.Tasks;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Extractors;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Extractors
{
    [TestFixture]
    public class SteamPostExtractorTest
    {
        [Test]
        public async Task Extract_Variant1_Post()
        {
            var target = new SteamPostExtractor();
            var actual = await target.ExtractAsync("https://store.steampowered.com/app/273750/Formula_Truck_2013/");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Formula Truck 2013", actual.Title);
            StringAssert.Contains(@"Windows - PC is the official simulator of the popular Brazilian", actual.Content);
            StringAssert.Contains(@"Light on system requirements", actual.Content);
             Assert.AreEqual(PostCategory.Game, actual.Category);

            var actualCompanies = actual.Companies.ToArray();
            Assert.AreEqual(1, actualCompanies.Length);
            Assert.AreEqual("Reiza Studios", actualCompanies[0]);

            var actualTags = actual.Tags.ToArray();
            Assert.AreEqual(8, actualTags.Length);
            CollectionAssert.Contains(actualTags, "reiza-studios");
            CollectionAssert.Contains(actualTags, "corrida");
            CollectionAssert.Contains(actualTags, "simulacao");
            CollectionAssert.Contains(actualTags, "esporte");
            CollectionAssert.Contains(actualTags, "multiplayer");
            CollectionAssert.Contains(actualTags, "windows");
            CollectionAssert.Contains(actualTags, "steam");
            CollectionAssert.Contains(actualTags, "press-release");

            Assert.AreEqual("https://steamcdn-a.akamaihd.net/steam/apps/273750/header.jpg?t=1447360130", actual.Logo);

            var actualScreenshots = actual.Screenshots.ToArray();
            Assert.AreEqual(8, actualScreenshots.Length);
            Assert.AreEqual("https://steamcdn-a.akamaihd.net/steam/apps/273750/ss_2395d05bdbb790b1a91c1ae48d00cc1c649c16d9.1920x1080.jpg?t=1447360130", actualScreenshots[0]);
            Assert.AreEqual("https://steamcdn-a.akamaihd.net/steam/apps/273750/ss_93cb92cf4691d8981d835f510fc5d6bcdd45929e.1920x1080.jpg?t=1447360130", actualScreenshots[7]);

            var actualVideos = actual.Videos.ToArray();
            Assert.AreEqual(0, actualVideos.Length);
        }

        [Test]
        public async Task Extract_Variant2_Post()
        {
            var target = new SteamPostExtractor();
            var actual = await target.ExtractAsync("https://store.steampowered.com/app/696140/No_Heroes_Here/");

            Assert.IsNotNull(actual);
        }
    }
}