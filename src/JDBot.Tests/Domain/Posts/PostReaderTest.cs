using System;
using NUnit.Framework;
using JDBot.Domain.Posts;
using NSubstitute;
using System.Threading.Tasks;

namespace JDBot.Tests.Domain.Posts
{
    [TestFixture]
    public class PostReaderTest
    {
        [Test]
        public async Task ReadAsync_TwoExtractors_Post()
        {
            var extractor1 = Substitute.For<IPostExtractor>();
            extractor1.ExtractAsync("testUrl").Returns((Post)null);

            var extractor2 = Substitute.For<IPostExtractor>();
            extractor2.ExtractAsync("testUrl").Returns(new Post
            {
                Title = "Test title",
                Content = "Test content",
                Category = PostCategory.Game
            });

            var extractors = new IPostExtractor[]
            {
                extractor1,
                extractor2
            };

            var target = new PostReader(extractors);
            var actual = await target.ReadAsync("testUrl");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Test title", actual.Title);
            Assert.AreEqual("Test content", actual.Content);
            Assert.AreEqual(PostCategory.Game, actual.Category);
        }
    }
}
