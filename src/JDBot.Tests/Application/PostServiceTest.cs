using System;
using System.IO;
using System.Threading.Tasks;
using JDBot.Application;
using JDBot.Domain.Posts;
using NUnit.Framework;

namespace JDBot.Tests.Application
{
    [TestFixture]
    public class PostServiceTest
    {
        [Test]
        public async Task WritePost_WordPressPostUrl_PostFilesWrite()
        {
            var jekyllRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jekyll");

            if(Directory.Exists(jekyllRootFolder))
                Directory.Delete(jekyllRootFolder, true);

            var target = new PostService(jekyllRootFolder);

            await target.WritePostAsync("http://www.monsterbed.com.br/games/trilha-ecologica/", new PostConfig());

            var now = DateTime.Now;
            var postFilename = Path.Combine(jekyllRootFolder, "_posts", now.Year.ToString(), $"{now.Year}-{now.Month:00}-{now.Day:00}-trilha-ecologica.md");
            FileAssert.Exists(postFilename);

            var postImageFolder = Path.Combine(jekyllRootFolder, "assets", now.Year.ToString(), now.Month.ToString("00"), now.Day.ToString("00"), "trilha-ecologica");
            DirectoryAssert.Exists(postImageFolder);

            Assert.AreEqual(11, Directory.GetFiles(postImageFolder, "*.jpg").Length);
        }
    }
}
