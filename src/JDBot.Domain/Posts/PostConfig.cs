using System;
namespace JDBot.Domain.Posts
{
    public class PostConfig
    {
        public DateTime? Date { get; set; }
        public string Author { get; set; }
        public int IgnoreImagesLowerThanBytes { get; set; } = 10000;
    }
}
