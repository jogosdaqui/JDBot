using System;
namespace JDBot.Domain.Posts
{
    public class PostConfig
    {
        public static readonly PostConfig Empty = new PostConfig();

        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string Author { get; set; }
        public int IgnoreImagesLowerThanBytes { get; set; } = 5120; 
    }
}
