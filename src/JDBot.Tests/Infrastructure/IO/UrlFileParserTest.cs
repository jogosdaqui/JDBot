using System;
using System.IO;
using System.Linq;
using JDBot.Infrastructure.IO;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.IO
{
    [TestFixture]
    public class UrlFileParserTest
    {
        [Test]
        public void Parse_FileName_Items()
        {
            var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure", "IO", "sample-url-file.txt");
            var actual = UrlFileParser.Parse(filename);
            var items = actual.Items.ToArray();

            Assert.AreEqual("/Users/giacomelli/Projects/jogosdaqui.github.io-jekyll/src", actual.JekyllRootFolder);
            Assert.AreEqual(3, items.Length);

            var item = items[0];
            Assert.AreEqual("http://www.monsterbed.com.br/games/trilha-ecologica/", item.Url);
             Assert.AreEqual("autor1", item.Config.Author);
            Assert.AreEqual(new DateTime(2018, 11, 28), item.Config.Date);

            item = items[1];
            Assert.AreEqual("http://www.monsterbed.com.br/games/isolados-a-fuga/", item.Url);
            Assert.AreEqual("autor2", item.Config.Author);
            Assert.AreEqual(new DateTime(2018, 11, 29), item.Config.Date);

            item = items[2];
            Assert.AreEqual("http://www.monsterbed.com.br/games/laco-macanudo-gaucho/", item.Url);
            Assert.AreEqual("autor3", item.Config.Author);
            Assert.AreEqual(new DateTime(2018, 12, 15), item.Config.Date);
        }
    }
}
