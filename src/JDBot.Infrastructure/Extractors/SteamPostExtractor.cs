using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.Extractors
{
    public class SteamPostExtractor : IPostExtractor
    {
        private static readonly Regex _getImageDocRegex = new Regex("href=\"(?<url>image.\\S+\\.(htm|html))\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex _getVideoDocRegex = new Regex("href=\"(?<url>video.\\S+\\.(htm|html))\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public async Task<Post> ExtractAsync(string url)
        {
            var doc = await $"{url}?l=brazilian".GetContentAsync();
            var contentElement = doc.QuerySelector(".game_area_description");
            var titleElement = doc.QuerySelector(".apphub_AppName");

            if (contentElement == null || titleElement == null)
                return null;

            var post = new Post();
            post.Content = contentElement.TextContent.Trim();
            post.Title = titleElement.TextContent.Trim();
            post.Category = PostCategory.Game;
            post.FillOriginalUrl(url);
            post.Companies = new String[] { doc.QuerySelector("#developers_list a").TextContent.Trim() };
            post.FillTags(doc);
            AddTagsFromSteamTags(doc, post);
            AddTagsFromSystemRequirements(doc, post);
            post.TranslateTags();

            post.Screenshots = doc.GetScreenshots(".highlight_screenshot_link");
            post.Logo = doc.QuerySelector(".game_header_image_full").Attributes["src"].Value;
            post.FillVideos(doc);

            return post;
        }

        private static void AddTagsFromSystemRequirements(IDocument doc, Post post)
        {
            var systemReq = doc.QuerySelector(".game_area_sys_req_leftCol, .game_area_sys_req").TextContent;

            if (systemReq.Contains("Windows"))
                post.Tags.Add("windows");

            if (systemReq.Contains("Mac"))
                post.Tags.Add("mac");

            if (systemReq.Contains("Linux"))
                post.Tags.Add("linux");
        }

        private static void AddTagsFromSteamTags(IDocument doc, Post post)
        {
            foreach (var tag in doc.QuerySelectorAll("a.app_tag").Select(t => t.TextContent.Trim().ToLowerInvariant()))
            {
                post.Tags.Add(tag);
            }
            post.Tags.Add("steam");
        }
    }
}
