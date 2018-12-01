using System.Linq;
using System.Threading.Tasks;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Extractors;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Extractors
{
    [TestFixture]
    public class DoPresskitPostExtractorTest
    {
        [Test]
        public async Task Extract_Variant1_Post()
        {
            var target = new DoPresskitVariant1PostExtractor();
            var actual = await target.ExtractAsync("http://www.firecaststudio.com/press/sheet.php?p=Jelly%20Dreams");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Jelly Dreams", actual.Title);
            StringAssert.Contains(@"Help Nino to solve the most", actual.Content);
            StringAssert.Contains(@"The game Jelly Dreams comes to virtual store", actual.Content);
            StringAssert.Contains(@"original soundtrack", actual.Content);
            Assert.AreEqual(PostCategory.Game, actual.Category);

            var actualCompanies = actual.Companies.ToArray();
            Assert.AreEqual(1, actualCompanies.Length);
            Assert.AreEqual("Firecast Studio", actualCompanies[0]);

            var actualTags = actual.Tags.ToArray();
            Assert.AreEqual(2, actualTags.Length);
            Assert.AreEqual("firecast-studio", actualTags[0]);
            Assert.AreEqual("press-release", actualTags[1]);


            Assert.AreEqual("http://www.firecaststudio.com/press/Jelly Dreams/images/logos/jellydreams-logo.png", actual.Logo);

            var actualScreenshots = actual.Screenshots.ToArray();
            Assert.AreEqual(9, actualScreenshots.Length);
            Assert.AreEqual("http://www.firecaststudio.com/press/Jelly Dreams/images/images/magic01.png", actualScreenshots[0]);
            Assert.AreEqual("http://www.firecaststudio.com/press/Jelly Dreams/images/images/magic02.png", actualScreenshots[1]);
            Assert.AreEqual("http://www.firecaststudio.com/press/Jelly Dreams/images/images/oldwest01.png", actualScreenshots[2]);
            Assert.AreEqual("http://www.firecaststudio.com/press/Jelly Dreams/images/images/oldwest02.png", actualScreenshots[3]);
            Assert.AreEqual("http://www.firecaststudio.com/press/Jelly Dreams/images/images/oldwest03.png", actualScreenshots[4]);
            Assert.AreEqual("http://www.firecaststudio.com/press/Jelly Dreams/images/images/thematicpacks.png", actualScreenshots[5]);
            Assert.AreEqual("http://www.firecaststudio.com/press/Jelly Dreams/images/images/treasure01.png", actualScreenshots[6]);
            Assert.AreEqual("http://www.firecaststudio.com/press/Jelly Dreams/images/images/treasure02.png", actualScreenshots[7]);
            Assert.AreEqual("http://www.firecaststudio.com/press/Jelly Dreams/images/images/treasure03.png", actualScreenshots[8]);

            var actualVideos = actual.Videos.ToArray();
            Assert.AreEqual(2, actualVideos.Length);
            Assert.AreEqual("lJnhD2itMeY", actualVideos[0].Id);
            Assert.AreEqual(VideoKind.YouTube, actualVideos[0].Kind);
            Assert.AreEqual("4cutOMSUv44", actualVideos[1].Id);
            Assert.AreEqual(VideoKind.YouTube, actualVideos[1].Kind);
        }

        [Test]
        public async Task Extract_Variant2_Post()
        {
            var target = new DoPresskitVariant2PostExtractor();
            var actual = await target.ExtractAsync("http://skahal.github.io/press/kit/snb/index.html");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Ships N' Battles", actual.Title);
            StringAssert.Contains(@"Play the next generation of the classic game", actual.Content);
            StringAssert.Contains(@"is the next generation of the popular pencil-and-paper game 'Battleship'", actual.Content);
            StringAssert.Contains(@"Accelerated mode in the single player", actual.Content);
            Assert.AreEqual(PostCategory.Game, actual.Category);

            var actualCompanies = actual.Companies.ToArray();
            Assert.AreEqual(1, actualCompanies.Length);
            Assert.AreEqual("Skahal Studios", actualCompanies[0]);

            var actualTags = actual.Tags.ToArray();
            Assert.AreEqual(5, actualTags.Length);
            Assert.AreEqual("skahal-studios", actualTags[0]);
            Assert.AreEqual("ios", actualTags[1]);
            Assert.AreEqual("mac", actualTags[2]);
            Assert.AreEqual("android", actualTags[3]);
            Assert.AreEqual("press-release", actualTags[4]);


            Assert.AreEqual("http://skahal.github.io/press/kit/snb/images/logoAndroid.png", actual.Logo);

            var actualScreenshots = actual.Screenshots.ToArray();
            Assert.AreEqual(16, actualScreenshots.Length);
            Assert.AreEqual("http://skahal.github.io/press/kit/snb/images/1_iPhone_01.png", actualScreenshots[0]);

            var actualVideos = actual.Videos.ToArray();
            Assert.AreEqual(2, actualVideos.Length);
            Assert.AreEqual("8qVJPZkMaRc", actualVideos[0].Id);
            Assert.AreEqual(VideoKind.YouTube, actualVideos[0].Kind);
            Assert.AreEqual("v7yMYS9WeCU", actualVideos[1].Id);
            Assert.AreEqual(VideoKind.YouTube, actualVideos[1].Kind);
        }
    }
}