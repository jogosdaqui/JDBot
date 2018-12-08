using System;
using System.IO;
using JDBot.Infrastructure.Framework;

namespace JDBot.Domain.Posts
{
    public class PostInfo
    {
        public static readonly PostInfo Empty = new PostInfo();

        private PostInfo()
        {

        }

        public PostInfo(string title, DateTime date)
        {
            if (String.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));

            var postName = GetPostName(title);
            FileName = Path.Combine("_posts", date.Year.ToString(), $"{date.Year}-{date.Month:00}-{date.Day:00}-{postName}.md");
            ImagesFolder = Path.Combine("assets", date.Year.ToString(), date.Month.ToString("00"), date.Day.ToString("00"), postName);
        }

        public string FileName { get; }
        public string ImagesFolder { get; }

        private static string GetPostName(string title)
        {
            return title
                       .ToLowerInvariant()
                       .Replace(" ", "-")
                       .Replace(":", "-")
                       .Replace("–", String.Empty)
                       .Replace("--", "-")
                       .Replace("'", String.Empty)
                       .RemoveDiacritics();

        }
    }
}
