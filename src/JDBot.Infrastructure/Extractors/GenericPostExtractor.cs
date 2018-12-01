using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    public class GenericPostExtractor : IPostExtractor
    {
        private static readonly Regex _getCompanyNameRegex = new Regex("Developer: (?<name>.+);", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public async Task<Post> ExtractAsync(string url)
        {
            var doc = await url.GetContentAsync();
          
            var post = new Post();
            post.Title = GetTitle(doc);
            post.Content = doc.QuerySelector("body").TextContent;
            post.Category = PostCategory.Game;
            post.FillOriginalUrl(url);
            post.FillVideo(doc);
            post.Companies = GetCompanies(post);
            post.FillTags(doc);
            post.Screenshots = doc.GetScreenshots("img", url);
            post.Logo = post.Screenshots.FirstOrDefault(p => p.Contains("logo"));
            post.Screenshots = post.Screenshots.Where(p => !p.Contains("logo")).ToArray();

            return post;
        }

        private IEnumerable<string> GetCompanies(Post post)
        {
            var match = _getCompanyNameRegex.Match(post.Content);

            if (match.Success)
                return new string[] { match.Groups["name"].Value };

            return new string[0];
        }

        private string GetTitle(IDocument doc)
        {
            return doc.Title
                      .Replace(" Press Kit", "")
                      .Replace("Press | ", "");
        }
    }
}
