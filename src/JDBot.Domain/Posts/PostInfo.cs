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

        public PostInfo(string jekyllRootFolder, string title, DateTime date)
        {
            if (String.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));

            Title = title;
            Date = date;
            var postName = GetPostName(title);

            jekyllRootFolder = jekyllRootFolder ?? String.Empty;
            FileName = Path.Combine(jekyllRootFolder, "_posts", date.Year.ToString(), $"{date.Year}-{date.Month:00}-{date.Day:00}-{postName}.md");
            ImagesFolder = Path.Combine(jekyllRootFolder, "assets", date.Year.ToString(), date.Month.ToString("00"), date.Day.ToString("00"), postName);
        }
      
        public PostInfo(string title, DateTime date)
            : this(null, title, date)
        {
        }

        public string Title { get; }
        public DateTime Date { get; }
        public string FileName { get; }
        public string ImagesFolder { get; }

        public static PostInfo From(Post post, string jekyllRootFolder)
        {
            return new PostInfo(jekyllRootFolder, post.Title, post.Date);
        }

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
