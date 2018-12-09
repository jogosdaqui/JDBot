using System;
using System.IO;
using JDBot.Domain.Posts;
using NUnit.Framework;

namespace JDBot.Tests.Domain.Posts
{
    [TestFixture]
    public class PostInfoTest
    {
        [Test]
        public void Constructor_TitleEmpty_Exception()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new PostInfo(String.Empty, DateTime.Now);
            });

            Assert.Catch<ArgumentNullException>(() =>
            {
                new PostInfo(null, DateTime.Now);
            });
        }

        [Test]
        public void FileName_TitleAndDate_Value()
        {
            var target = new PostInfo("título de teste", new DateTime(2018, 12, 8));
            Assert.AreEqual(Path.Combine("_posts", "2018", "2018-12-08-titulo-de-teste.md"), target.FileName);
        }

        [Test]
        public void ImagesFolder_TitleAndDate_Value()
        {
            var target = new PostInfo("título de teste", new DateTime(2018, 12, 8));
            Assert.AreEqual(Path.Combine("assets", "2018", "12", "08", "titulo-de-teste"), target.ImagesFolder);
        }
    }
}
