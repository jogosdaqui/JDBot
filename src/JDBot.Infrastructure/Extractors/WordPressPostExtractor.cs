using System.Threading.Tasks;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    public class WordPressPostExtractor : IPostExtractor
    {
        public async Task<Post> ExtractAsync(string url)
        {
            var doc = await url.GetContentAsync();

            var title = doc.QuerySelector("h1");
            var contents = doc.QuerySelectorAll(".article-content p");

            // É um post do WordPress?
            if (title == null || contents.Length == 0)
                return null;

            var post = new Post();
            post.Title = title.TextContent;
            post.Content = contents.JoinText();
            post.Category = url.Contains("game") ? PostCategory.Game : PostCategory.News;
            post.FillOriginalUrl(url);
            post.FillTags(doc);
            post.FillCompanyFromTitle(doc);
            post.FillVideo(doc);
            post.Logo = doc.GetLogo(".wp-post-image");
            post.Screenshots = doc.GetScreenshots(".size-thumbnail");

            return post;
        }
    }
}