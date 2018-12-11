using System.Linq;
using System.Threading.Tasks;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Extractors;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Extractors
{
    [TestFixture]
    public class JogosdaquiPostExtractorTest
    {
        [Test]
        public async Task Extract_Url1_Post()
        {
            var target = new JogosdaquiPostExtractor();
            var actual = await target.ExtractAsync("https://jogosdaqui.github.io/2018/12/08/uga-buga-dino-rush");
            Assert.NotNull(actual);
            Assert.AreEqual("Uga Buga: Dino Rush", actual.Title);
            StringAssert.Contains("O Sr. Uga e a Sra. Buga precisam correr para encarar os perigos da Era das Cavernas.", actual.Content);
            StringAssert.Contains("Um jogo 100% brasileiro que vai agradar toda a família com certeza!", actual.Content);
            Assert.AreEqual("https://jogosdaqui.github.io/assets/2018/12/08/uga-buga-dino-rush/logo.jpg", actual.Logo);

            var actualScreenshots = actual.Screenshots.ToArray();
            Assert.AreEqual(7, actualScreenshots.Length);

            var actualVideos = actual.Videos.ToArray();
            Assert.AreEqual(1, actualVideos.Length);
            Assert.AreEqual("HlYOOs8JXSw", actualVideos[0].Id);
            Assert.AreEqual(VideoKind.YouTube, actualVideos[0].Kind);

            var actualTags = actual.Tags.ToArray();
            Assert.AreEqual(4, actualTags.Length);
            Assert.AreEqual("dead-mushroom", actualTags[0]);
            Assert.AreEqual("android", actualTags[1]);
            Assert.AreEqual("runner", actualTags[2]);
            Assert.AreEqual("press-release", actualTags[3]);
        }
    }
}
