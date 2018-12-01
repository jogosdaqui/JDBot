using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    /// <summary>
    /// Variante de extatror para posts presskit() como :
    /// * http://skahal.github.io/press/kit/snb/index.html
    /// * https://www.vlambeer.com/press/sheet.php?p=LUFTRAUSERS
    /// </summary>
    public class DoPresskitVariant2PostExtractor : IPostExtractor
    {
        private static readonly Regex _getGameTitleRegex = new Regex(@"(?<name>.+)\sCredits$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _getRootUrlRegex = new Regex(@"(?<baseUrl>.+)(/index.html|sheet.php\?p=)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public async Task<Post> ExtractAsync(string url)
        {
            var doc = await url.GetContentAsync();
            var title = doc.QuerySelector("h2[id='credits']");
            var content = doc.QuerySelector("article.description,.uk-width-medium-4-6");
            var images = doc.QuerySelectorAll("div[id='gallery'] img,h2[id='images']~div img");
            var company = doc.QuerySelector("h2[id='about']");

            // É um post do Presskit().
            if (title == null || content == null || images.Length == 0 || company == null)
                return null;

            var post = new Post();
            post.Title = _getGameTitleRegex.Replace(title.TextContent, "$1");
            post.Content = content.TextContent;
            post.Category = PostCategory.Game;
            post.FillOriginalUrl(url);
            post.FillVideo(doc);

            var companyName = company.TextContent.Replace("About ", string.Empty);
            post.Companies = new string[] { companyName };
            post.FillTags(doc);

            var baseUrl = _getRootUrlRegex.Match(url).Groups["baseUrl"].Value;
            post.Logo = doc.GetLogo("h2[id='logo']~a img", baseUrl);
            post.Screenshots = doc.GetScreenshots(images, baseUrl);

            return post;
        }
    }
}
