using System;
using System.Linq;
using System.Threading.Tasks;
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
                post.Logo = post.Screenshots.FirstOrDefault(p => p.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1);

                if (!string.IsNullOrEmpty(post.Logo))
                {
                    post.Screenshots = post.Screenshots.Where(p => !p.Equals(post.Logo, StringComparison.OrdinalIgnoreCase)).ToArray();
                    break;
                }
            }

            return post;
        }
    }
}
