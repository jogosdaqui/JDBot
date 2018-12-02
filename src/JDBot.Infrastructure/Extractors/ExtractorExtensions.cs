using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Texts;

namespace JDBot.Infrastructure.Extractors
{
    public static class ExtractorExtensions
    {
        private static readonly Regex _getVimeoIdRegex = new Regex(@"vimeo.com/video/(?<id>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _getYoutubeIdRegex = new Regex(@"https*://www.youtube.com/(watch\?v=|embed/)(?<id>[a-z0-9\-_]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _removeSizeFromImageUrlRegex = new Regex(@"\-\d+x\d+", RegexOptions.Compiled);
        private static readonly Regex _removePageRegex = new Regex(@"(?<baseUrl>.+)/.+\.(html|htm|php|aspx)$", RegexOptions.Compiled);

        private static readonly RegexFile _titleRegex = new RegexFile("Extractors/TitleRegex.json");
        private static readonly RegexFile _companyRegex = new RegexFile("Extractors/CompanyRegex.json");
        private static readonly RegexFile _tagRegex = new RegexFile("Extractors/TagRegex.json");

        public static void FillTags(this Post post, IDocument doc)
        {
            var tags = new List<string>();

            foreach (var company in post.Companies)
            {
                tags.Add(SanitizeTag(company));
            }
       
            var content = post.Content;
             tags.AddRange(_tagRegex.GetResponses(content));

            foreach (var item in doc.QuerySelectorAll("dd"))
            {
                tags.AddRange(_tagRegex.GetResponses(item.InnerHtml));
            }

            tags.Add("press-release");

            post.Tags = tags;
        }

        private static string SanitizeTag(string value)
        {
            return value.ToLowerInvariant().Replace(" ", "-");
        }

        public static void FillTitle(this Post post, IDocument doc)
        {
            post.Title = _titleRegex.GetValueByTag("text", "title", doc.Title, doc.Body.TextContent);

            if (String.IsNullOrEmpty(post.Title))
                post.Title = _titleRegex.GetValueByTag("html", "title", doc.Head.OuterHtml, doc.Body.OuterHtml);

            if (String.IsNullOrEmpty(post.Title))
                post.Title = doc.Title;
        }

        public static void FillCompanies(this Post post, IDocument doc)
        {
            var match = _companyRegex.MatchByTag("text", doc.Title, doc.Body.TextContent);

            if (!match.Success)
                match = _companyRegex.MatchByTag("html", doc.Head.OuterHtml, doc.Body.OuterHtml);

            if (match.Success)
                 post.Companies = new string[] { match.Groups["name"].Value.Trim() };
            else
                post.Companies = new string[0];
        }

        public static void FillVideos(this Post post, IDocument doc)
        {
            var videos = new List<Video>();

            var vimeos = doc.QuerySelectorAll("iframe[src*='player.vimeo.com']");

            foreach(var link in vimeos)
            {
                var idMatch = _getVimeoIdRegex.Match(link.OuterHtml);

                if (idMatch.Success)
                    videos.Add(new Video { Id = idMatch.Groups["id"].Value, Kind = VideoKind.Vimeo });
            }

            var youtubes = doc.QuerySelectorAll("a[href*='://www.youtube.com/'],iframe[src*='://www.youtube.com/']");

            foreach (var link in youtubes)
            {
                var idMatch = _getYoutubeIdRegex.Match(link.OuterHtml);

                if (idMatch.Success)
                     videos.Add(new Video { Id = idMatch.Groups["id"].Value, Kind = VideoKind.YouTube });
            }

            post.Videos = videos.GroupBy(k => k.Id, e => e.Kind).Select(g => new Video { Id = g.Key, Kind = g.First() }).ToArray();
        }

        public static void FillOriginalUrl(this Post post, string originalUrl)
        {
            var regex = new Regex(Regex.Escape(post.Title));

            post.Content = regex.Replace(post.Content, $"[{post.Title}]({originalUrl})", 1);
        }

        public static async Task<IDocument> GetContentAsync(this string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            return await BrowsingContext.New(config).OpenAsync(url);
        }

        public static string JoinText(this IHtmlCollection<IElement> elements, int limit = int.MaxValue)
        {
            var content = new StringBuilder();

            foreach (var p in elements)
            {
                content.AppendLine(p.TextContent);

                if (--limit == 0)
                    break;
            }

            return content.ToString();
        }

        public static string GetLogo(this IDocument doc, string selector, string baseUrl = null)
        {
            var logo = doc.QuerySelector(selector);

            return logo == null ? null : AddBaseUrl(baseUrl, logo.Attributes["src"].Value);
        }

        public static IEnumerable<string> GetScreenshots(this IDocument doc, string selector, string baseUrl = null)
        {
            return doc.GetScreenshots(doc.QuerySelectorAll(selector), baseUrl);
        }

        public static IEnumerable<string> GetScreenshots(this IDocument doc, IHtmlCollection<IElement> screenshotsElements, string baseUrl = null)
        {
            return screenshotsElements.Select(s => AddBaseUrl(baseUrl, _removeSizeFromImageUrlRegex.Replace(s.Attributes["src"].Value, "")));
        }

        private static string AddBaseUrl(this string baseUrl, string relativeUrl)
        {
            if (!String.IsNullOrEmpty(baseUrl))
                baseUrl = _removePageRegex.Replace(baseUrl, "${baseUrl}");

            if (String.IsNullOrEmpty(baseUrl) || String.IsNullOrEmpty(relativeUrl) || relativeUrl.Contains("http"))
                return relativeUrl;
             else
                return relativeUrl.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? $"{baseUrl}{relativeUrl}" : $"{baseUrl}/{relativeUrl}";
        }
    }
}
