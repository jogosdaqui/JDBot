using System.Linq;
using System.Threading.Tasks;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    public class JogosdaquiPostExtractor : IPostExtractor
    {
        public async Task<Post> ExtractAsync(string url)
        {
            var doc = await url.GetContentAsync();
          
            var post = new Post();
            post.Content = doc.QuerySelector(".entry").TextContent.Trim();
            post.Title = doc.QuerySelector("article h1").TextContent.Trim();
            post.Category = PostCategory.Game;
            post.FillOriginalUrl(url);
            post.FillVideos(doc);
            post.Companies = doc.QuerySelectorAll(".company-name").Select(t => t.TextContent.Trim()).ToList();
            post.Tags = doc.QuerySelectorAll(".post-tag").Select(t => t.TextContent.Trim()).ToList();
            post.Screenshots = doc.GetScreenshots("li img", "https://jogosdaqui.github.io");
            post.Logo = doc.GetLogo(".entry center > img", "https://jogosdaqui.github.io");    
                  
            return post;
        }
    }
}
