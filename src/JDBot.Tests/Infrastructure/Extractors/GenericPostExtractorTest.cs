using System.Linq;
using System.Threading.Tasks;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Extractors;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Extractors
{
    [TestFixture]
    public class GenericPostExtractorTest
    {
        [Test]
        public async Task Extract_Url1_Post()
        {
            var target = new GenericPostExtractor();
            var actual = await target.ExtractAsync("http://www.moiragame.com/presskit.html");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Möira", actual.Title);
            StringAssert.Contains("Here you should find any information", actual.Content);
            StringAssert.Contains("distorting the limitations and break the rules of the land", actual.Content);
            Assert.AreEqual(PostCategory.Game, actual.Category);

            var actualCompanies = actual.Companies.ToArray();
            Assert.AreEqual(1, actualCompanies.Length);
            Assert.AreEqual("Onagro Studios", actualCompanies[0]);

            var actualTags = actual.Tags.ToArray();
            Assert.AreEqual(5, actualTags.Length);
            Assert.AreEqual("onagro-studios", actualTags[0]);
            Assert.AreEqual("windows", actualTags[1]);
            Assert.AreEqual("mac", actualTags[2]);
            Assert.AreEqual("linux", actualTags[3]);
            Assert.AreEqual("press-release", actualTags[4]);

            Assert.AreEqual("http://www.moiragame.com/images/presskit/thumbs/moira_logo_imgt.png", actual.Logo);

            var actualScreenshots = actual.Screenshots.ToArray();
            Assert.AreEqual(14, actualScreenshots.Length);
            Assert.AreEqual("http://www.moiragame.com/images/header.png", actualScreenshots[0]);

            var actualVideos = actual.Videos.ToArray();
            Assert.AreEqual(1, actualVideos.Length);
            Assert.AreEqual("iXGRVygXDBU", actualVideos[0].Id);
            Assert.AreEqual(VideoKind.YouTube, actualVideos[0].Kind);
        }

        [Test]
        public async Task Extract_Url2_Post()
        {
            var target = new GenericPostExtractor();
            var actual = await target.ExtractAsync("http://www.pixelripped.com/presskit/index.html");

            var actualScreenshots = actual.Screenshots.ToArray();
            Assert.AreEqual(12, actualScreenshots.Length);
            Assert.AreEqual("http://www.pixelripped.com/presskit/images/screenshot0.jpg", actualScreenshots[2]);
        }

        [Test]
        public async Task Extract_Url3_Post()
        {
            var target = new GenericPostExtractor();
            var actual = await target.ExtractAsync("http://presskit.swordlegacy.com/");
            Assert.AreEqual("Sword Legacy: Omen", actual.Title);

            var actualScreenshots = actual.Screenshots.ToArray();
            Assert.AreEqual(14, actualScreenshots.Length);
         
            Assert.AreEqual(6, actual.Videos.Count());
        }

        [Test]
        public async Task Extract_Url4_Post()
        {
            var target = new GenericPostExtractor();
            var actual = await target.ExtractAsync("http://www.deadmushroom.com.br/?portfolio=amazing-spider-attack");
            Assert.IsFalse(string.IsNullOrEmpty(actual.Logo));
        }

        [Test]
        public async Task Extract_Url5_Post()
        {
            var target = new GenericPostExtractor();
            var actual = await target.ExtractAsync("http://www.invent4.com/rats/index-p.htm");
             Assert.AreEqual("http://www.invent4.com/rats/logo-preto.gif", actual.Logo);

            StringAssert.Contains("Quando os ratos", actual.Content);

            var actualScreenshots = actual.Screenshots.ToArray();
            Assert.AreEqual(34, actualScreenshots.Length);

            var actualVideos = actual.Videos.ToArray();
            Assert.AreEqual(3, actualVideos.Length);
            Assert.AreEqual("IAu4oJzzNDI", actualVideos[0].Id);
            Assert.AreEqual("WNj67cSF_zg", actualVideos[1].Id);
            Assert.AreEqual("P21xzCgtfCQ", actualVideos[2].Id);
        }
    }
}
