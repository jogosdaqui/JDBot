using System;

namespace JDBot.Domain.Videos
{
    public class VideoProducerOptions
    {
        public TimeSpan DurationPerUrl { get; set; } = TimeSpan.FromSeconds(10);
        public bool UseLogo { get; set; }
        public int MaxScreenshots { get; set; }
        public bool UseVideo { get; set; }
    }
}
