using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    public class GenericPostExtractor : IPostExtractor
    {
        private static readonly Regex _getImageDocRegex = new Regex("href=\"(?<url>image.\\S+\\.(htm|html))\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _getVideoDocRegex = new Regex("href=\"(?<url>video.\\S+\\.(htm|html))\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public async Task<Post> ExtractAsync(string url)
        {
            var doc = await url.GetContentAsync();
          
            var post = new Post();
            post.Content = doc.QuerySelector("body").TextContent.Trim();
            post.FillTitle(doc);
            post.Category = PostCategory.Game;
            post.FillOriginalUrl(url);
            post.FillVideos(await doc.GetReferencedDocAsync(_getVideoDocRegex));
            post.FillCompanies(doc);
            post.FillTags(doc);

            var imagesDoc = await doc.GetReferencedDocAsync(_getImageDocRegex);
            post.Screenshots = imagesDoc.GetScreenshots("img,a[href*='.jpg']", url);

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
