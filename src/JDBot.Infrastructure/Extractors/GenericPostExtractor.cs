using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    public class GenericPostExtractor : IPostExtractor
    {
        public async Task<Post> ExtractAsync(string url)
        {
            var doc = await url.GetContentAsync();
          
            var post = new Post();
            post.Content = doc.QuerySelector("body").TextContent.Trim();
            post.FillTitle(doc);
            post.Category = PostCategory.Game;
            post.FillOriginalUrl(url);
            post.FillVideos(doc);
            post.FillCompanies(doc);
            post.FillTags(doc);
            post.Screenshots = doc.GetScreenshots("img", url);

            foreach (var filter in new string[] { "logo", "avatar", "icon" })
            {
                post.Logo = post.Screenshots.FirstOrDefault(p => p.Contains(filter));

                if (!string.IsNullOrEmpty(post.Logo))
                {
                    post.Screenshots = post.Screenshots.Where(p => !p.Contains(filter)).ToArray();
                    break;
                }
            }

            return post;
        }
    }
}
