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

        /// <summary>
        /// Obtém ou define a data de lançamento do jogo.
        /// </summary>
        /// <remarks>
        /// Apenas para posts da categoryia Game.
        /// </remarks>
        public DateTime? ReleaseDate { get; set; }
        public IEnumerable<string> Companies { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public string Logo { get; set; }
        public IEnumerable<string> Screenshots { get; set; }
        public IEnumerable<Video> Videos { get; set; }
    }
}
