namespace JDBot.Domain.Videos
{
    public class VideoProducerOutput
    {
        public VideoProducerOutput(string videoFileName)
        {
            VideoFileName = videoFileName;
        }

        public string VideoFileName { get; }
    }
}
