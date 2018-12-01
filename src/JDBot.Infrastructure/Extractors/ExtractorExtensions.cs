using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    public static class ExtractorExtensions
    {
        private static readonly Regex _getCompanyNameFromTitleRegex = new Regex(@".+\| (?<name>.+)", RegexOptions.Compiled);
        private static readonly Regex _getVimeoIdRegex = new Regex(@"vimeo.com/video/(?<id>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _getYoutubeIdRegex = new Regex(@"https*://www.youtube.com/(watch\?v=|embed/)(?<id>[a-z0-9\-_]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _removeSizeFromImageUrlRegex = new Regex(@"\-\d+x\d+", RegexOptions.Compiled);
        private static readonly Regex _removePageRegex = new Regex(@"(?<baseUrl>.+)/.+\.(html|htm|php|aspx)$", RegexOptions.Compiled);

        public static void FillTags(this Post post, IDocument doc)
        {
            var tags = new List<string>();
            var companyNameMatch = _getCompanyNameFromTitleRegex.Match(doc.Title);

            if (companyNameMatch.Success)
                tags.Add(SanitizeTag(companyNameMatch.Groups["name"].Value));
            else
            {
                foreach (var company in post.Companies)
                {
                    tags.Add(SanitizeTag(company));
                }
            }

            var content = post.Content;

            if (content.Contains("público alvo criança"))
                tags.Add("infantil");

            if (content.Contains("jogo plataforma"))
                tags.Add("plataforma");

            if (content.Contains("publicado para Facebook, Android e iOS"))
            {
                tags.Add("facebook");
                tags.Add("android");
                tags.Add("ios");
            }

            if (content.Contains("PC, MAC and Linux") || content.Contains("Windows, Mac, Linux"))
            {
                tags.Add("windows");
                tags.Add("mac");
                tags.Add("linux");
            }

            if (content.Contains("player.vimeo.com"))
            {
                tags.Add("video");
            }

            foreach (var item in doc.QuerySelectorAll("dd"))
            {
                if (item.InnerHtml.Contains(">iOS"))
                    tags.Add("ios");

                if (item.InnerHtml.Contains(">Android"))
                    tags.Add("android");

                if (item.InnerHtml.Contains(">Mac"))
                    tags.Add("mac");

                if (item.InnerHtml.Contains(">PC"))
                    tags.Add("windows");

            }

            tags.Add("press-release");

            post.Tags = tags;
        }

        private static string SanitizeTag(string value)
        {
            return value.ToLowerInvariant().Replace(" ", "-");
        }

        public static void FillCompanyFromTitle(this Post post, IDocument doc)
        {
            var companyNameMatch = _getCompanyNameFromTitleRegex.Match(doc.Title);

            if (companyNameMatch.Success)
                post.Companies = new string[] { companyNameMatch.Groups["name"].Value };
        }

        public static void FillVideo(this Post post, IDocument doc)
        {
            var videos = new List<Video>();

            // <iframe width="940" height="460" src="//player.vimeo.com/video/84909758?color=C0091C" frameborder="0" allowfullscreen=""></iframe>
            var vimeos = doc.QuerySelectorAll("iframe[src*='player.vimeo.com']");

            foreach(var link in vimeos)
            {
                var idMatch = _getVimeoIdRegex.Match(link.OuterHtml);

                if (idMatch.Success)
                {
                    post.Content += $"\n\n{{% vimeo {idMatch.Groups["id"].Value} %}}";
                    videos.Add(new Video { Id = idMatch.Groups["id"].Value, Kind = VideoKind.Vimeo });
                }
            }

            // <a href="https://www.youtube.com/watch?v=JpW7PFCJv1E" title="Assista o Gameplay" target="_blank">Assista o Gameplay</a>
            // <a href="https://www.youtube.com/embed/8qVJPZkMaRc?rel=0">YouTube</a>
            // https://www.youtube.com/embed/iXGRVygXDBU
            var youtubes = doc.QuerySelectorAll("a[href*='://www.youtube.com/'],iframe[src*='://www.youtube.com/']");

            foreach (var link in youtubes)
            {
                var idMatch = _getYoutubeIdRegex.Match(link.OuterHtml);

                if (idMatch.Success)
                {
                    post.Content += $"\n\n{{% youtube {idMatch.Groups["id"].Value} %}}";
                    videos.Add(new Video { Id = idMatch.Groups["id"].Value, Kind = VideoKind.YouTube });
                }
            }

            post.Videos = videos.GroupBy(k => k.Id, e => e.Kind).Select(g => new Video { Id = g.Key, Kind = g.First() });
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
