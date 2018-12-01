namespace JDBot.Infrastructure.Framework
{
    public class ImageResource
    {
        public ImageResource() { }

        public ImageResource(byte[] data)
        {
            Data = data;
        }

        public ImageResource(byte[] data, string extension)
            : this(data)
        {
            Extension = extension;
        }

        public byte[] Data { get; set; }
        public string Extension { get; set; }
    }
}
