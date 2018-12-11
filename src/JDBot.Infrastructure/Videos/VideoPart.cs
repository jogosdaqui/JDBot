using System;
using JDBot.Infrastructure.Framework;

namespace JDBot.Infrastructure.Videos
{
    internal class VideoPart
    {
        public string Text { get; set; }
        public ImageResource Image { get; set; }
         public TimeSpan Start { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
