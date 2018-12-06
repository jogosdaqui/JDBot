using System;
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

            var post = new Post();
            post.Content = doc.QuerySelector("[jsname='sngebd']").TextContent.Trim();
            post.Title = doc.QuerySelector("meta[itemprop='name']").Attributes["content"].Value.Trim();
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
