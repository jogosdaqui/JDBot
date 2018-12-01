using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    /// <summary>
    /// Variante de extrator para posts presskit() como http://www.firecaststudio.com/press/sheet.php?p=Jelly%20Dreams.
    /// </summary>
    public class DoPresskitVariant1PostExtractor : IPostExtractor
    {
        private static readonly Regex _getCompanyUrlRegex = new Regex("/sheet.php.+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public async Task<Post> ExtractAsync(string url)
        {
            var doc = await url.GetContentAsync();
            var content = doc.QuerySelector(".block02");
            var images = doc.QuerySelectorAll("h2[id='imagens'] + .imglist img");

            // É um post do Presskit().
            if (content == null || images.Length == 0)
                return null;

            var post = new Post();
            post.Content = content.TextContent;
            post.Title = doc.Title;
            post.Category = PostCategory.Game;
            post.FillOriginalUrl(url);
            post.FillVideos(doc);
           
            var companyUrl = _getCompanyUrlRegex.Replace(url, String.Empty);
            var companyDoc = await companyUrl.GetContentAsync();
            post.Companies = new string[] { companyDoc.Title };
            post.FillTags(doc);

            post.Logo = doc.GetLogo("h2[id='logos'] + ul img[src*='logo']:not([src*='x'])", companyUrl);
            post.Screenshots = doc.GetScreenshots(images, companyUrl);

            return post;
        }
    }
}
