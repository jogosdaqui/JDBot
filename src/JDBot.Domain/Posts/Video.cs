using System.Diagnostics;

namespace JDBot.Domain.Posts
{
    [DebuggerDisplay("{Kind} {Id}")]
    public class Video
    {
        public string Id { get; set; }
        public VideoKind Kind { get; set; }
    }
}
