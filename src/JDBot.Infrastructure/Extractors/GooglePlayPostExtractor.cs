using System.Linq;
using System.Threading.Tasks;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    public class GooglePlayPostExtractor : IPostExtractor
    {
        public async Task<Post> ExtractAsync(string url)
        {
            // Adiciona o "&hl=pt" para tentar obter o texto em pt-BR.
            var doc = await $"{url}&hl=pt".GetContentAsync();
            var contentElement = doc.QuerySelector("[jsname='sngebd']");
            var titleElement = doc.QuerySelector("meta[itemprop='name']");

            if (contentElement == null || titleElement == null)
                return null;

            var post = new Post();
            post.Content = contentElement.TextContent.Trim();
            post.Title = titleElement.Attributes["content"].Value.Trim();
            post.Category = PostCategory.Game;
            post.FillOriginalUrl(url);
            post.FillVideos(doc);
            post.FillCompanies(doc);
            post.FillTags(doc);
            post.Tags.Insert(1, "android");
            post.Screenshots = doc
                                .QuerySelectorAll("[itemprop='image']")
                                .Where(e => e.HasAttribute("src"))
                                .Select(e => e.Attributes["src"].Value.Trim()).ToList();
            post.Logo = post.Screenshots[0];
            post.Screenshots.RemoveAt(0);


            return post;
        }
    }
}
