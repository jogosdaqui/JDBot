namespace JDBot.Domain.Posts
{
    public class PostWrittenResult
    {
        public static readonly PostWrittenResult Empty = new PostWrittenResult(null, null);

        public PostWrittenResult(string fileName, string imagesFolder)
        {
            FileName = fileName;
            ImagesFolder = imagesFolder;
        }
       
        public string FileName { get; }
        public string ImagesFolder { get; }
    }
}
