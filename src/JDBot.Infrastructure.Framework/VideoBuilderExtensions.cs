using System;
namespace JDBot.Infrastructure.Framework
{
    public static class VideoBuilderExtensions
    {
        public static IVideoBuilder AddImage(this IVideoBuilder builder, ImageResource image, int durationSeconds)
        {
            return builder.AddImage(image, TimeSpan.FromSeconds(durationSeconds));
        }
    }
}
