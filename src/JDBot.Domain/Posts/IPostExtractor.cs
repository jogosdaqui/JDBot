using System;
using System.Threading.Tasks;

namespace JDBot.Domain.Posts
{
    public interface IPostExtractor
    {
        Task<Post> ExtractAsync(string url);
    }
}
