using System;
namespace JDBot.Infrastructure.Framework
{
    public interface IVideoBuilder
    {
        IVideoBuilder AddTitle(string text, TimeSpan duration);
        IVideoBuilder AddDescription(string text, TimeSpan duration);
        IVideoBuilder AddImage(ImageResource image, TimeSpan duration);

        string Build();
    }
}
