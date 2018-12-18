using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    public class WordPressPostExtractor : IPostExtractor
    {
        private static readonly Regex _removeSizeFromImageUrlRegex = new Regex(@"\-\d+x\d+", RegexOptions.Compiled);

        public async Task<Post> ExtractAsync(string url)
        {
            var doc = await url.GetContentAsync();

            var title = doc.QuerySelector("h1");
            var contents = doc.QuerySelectorAll(".article-content p");

            // É um post do WordPress?
            if (title == null || contents.Length == 0)
                return null;

            var post = new Post();
            post.Content = contents.JoinText();
            post.Title = title.TextContent;
            post.Category = url.Contains("game") ? PostCategory.Game : PostCategory.News;
            post.FillOriginalUrl(url);
            post.FillCompanies(doc);
            post.FillTags(doc);
            post.FillVideos(doc);
            post.Logo = doc.GetLogo(".wp-post-image");
            post.Screenshots = doc
                                    .GetScreenshots(".size-thumbnail")
                                    .Select(s => _removeSizeFromImageUrlRegex.Replace(s, string.Empty))
                                    .ToList();

            return post;
        }
    }
}