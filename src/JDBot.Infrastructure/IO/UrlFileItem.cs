using JDBot.Domain.Posts;

namespace JDBot.Infrastructure.IO
{
    public class UrlFileItem
    {
        public string Url { get; set;}
        public PostConfig Config { get; set; }
    }
}
