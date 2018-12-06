using System;
using System.Collections.Generic;

namespace JDBot.Domain.Posts
{
    public class Post
    {
        public string Title { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Content { get; set; }
        public PostCategory Category { get; set; }
        public string Author { get; set; }

        /// <summary>
        /// Obtém ou define a data de lançamento do jogo.
        /// </summary>
        /// <remarks>
        /// Apenas para posts da categoryia Game.
        /// </remarks>
        public DateTime? ReleaseDate { get; set; }
        public IList<string> Companies { get; set; }
        public IList<string> Tags { get; set; }
        public string Logo { get; set; }
        public IList<string> Screenshots { get; set; }
        public IList<Video> Videos { get; set; }
    }
}
