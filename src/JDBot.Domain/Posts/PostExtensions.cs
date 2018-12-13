using System;
using JDBot.Infrastructure.Framework;

namespace JDBot.Domain.Posts
{
    public static class PostExtensions
    {
        public static string GetWritableName(this Post post)
        {
            return GetWritetablePostName(post.Title);

        }

        public static string GetWritetablePostName(this string title)
        {
            return title
                   .ToLowerInvariant()
                   .Replace(" ", "-")
                   .Replace(":", "-")
                   .Replace("–", String.Empty)
                   .Replace("'", String.Empty)
                   .Replace("#", String.Empty)
                   .Replace(",", "-")
                   .Replace("?", "-")
                   .Replace("!", "-")
                   .Replace("--", "-")
                   .RemoveDiacritics();
        }

        public static string GetWritableTitle(this Post post)
        {
            return post.Title.Replace("'", "&#39;");
        }
    }
}

