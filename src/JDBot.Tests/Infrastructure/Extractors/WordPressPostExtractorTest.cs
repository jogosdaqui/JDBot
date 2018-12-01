using System;
using System.Linq;
using System.Threading.Tasks;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Extractors;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Extractors
{
    [TestFixture]
    public class WordPressPostExtractorTest
    {
        [Test]
        public async Task Extract_IsSimplePageUrl_Post()
        {
            var target = new WordPressPostExtractor();
            var actual = await target.ExtractAsync("http://www.monsterbed.com.br/games/trilha-ecologica/");

            Assert.IsNotNull(actual);
            Assert.AreEqual("Trilha Ecológica", actual.Title);
            Assert.AreEqual("[Trilha Ecológica](http://www.monsterbed.com.br/games/trilha-ecologica/) foi desenvolvido pela MonsterBed em parceria com a Empresa Brasileira de Pesquisa Agropecuária (Embrapa), uma instituição pública de pesquisa vinculada ao Ministério da Agricultura.\nO jogo, que tem como público alvo crianças de 8 a 12 anos, visa ensinar a importância da preservação da natureza. Isso acontece através de uma trilha ecológica da Embrapa, onde o jogador deverá percorrê-la aprendendo a importância de mantê-la limpa e conhecendo as árvores nativas que a compõe.\nÉ um jogo plataforma 3D com temática educacional, onde o jogador irá escolher entre 2 personagens: Teo ou Neila. Depois, viverá uma grande aventura pela trilha, recolhendo o lixo encontrado e enfrentando inimigos. No fim, terá que enfrentar o temível vilão de lixo para salvar seu amigo.\n", actual.Content);
            Assert.AreEqual(PostCategory.Game, actual.Category);

            var actualCompanies = actual.Companies.ToArray();
            Assert.AreEqual(1, actualCompanies.Length);
            Assert.AreEqual("Monsterbed Game Studio", actualCompanies[0]);

            var actualTags = actual.Tags.ToArray();
            Assert.AreEqual(4, actualTags.Length);
            Assert.AreEqual("monsterbed-game-studio", actualTags[0]);
            Assert.AreEqual("infantil", actualTags[1]);
            Assert.AreEqual("plataforma", actualTags[2]);
            Assert.AreEqual("press-release", actualTags[3]);

            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/960x460_-Embrapa-Entrada.png", actual.Logo);

            var actualScreenshots = actual.Screenshots.ToArray();
            Assert.AreEqual(10, actualScreenshots.Length);
            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/embrapa1.jpg", actualScreenshots[0]);
            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/embrapa2.jpg", actualScreenshots[1]);
            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/embrapa3.jpg", actualScreenshots[2]);
            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/embrapa10.jpg", actualScreenshots[3]);
            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/embrapa4.jpg", actualScreenshots[4]);
            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/embrapa6.jpg", actualScreenshots[5]);
            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/embrapa5.jpg", actualScreenshots[6]);
            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/embrapa7.jpg", actualScreenshots[7]);
            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/embrapa9.jpg", actualScreenshots[8]);
            Assert.AreEqual("http://www.monsterbed.com.br/wp-content/uploads/2016/08/embrapa8.jpg", actualScreenshots[9]);  
        }

        [Test]
        public async Task Extract_WithVimeoVide_PostWithVimeoTag()
        {
            var target = new WordPressPostExtractor();
            var actual = await target.ExtractAsync("http://www.monsterbed.com.br/games/laco-macanudo-gaucho/");

            Assert.IsNotNull(actual);
            StringAssert.Contains("{% vimeo 84909758 %}", actual.Content);
        }
    }
}
