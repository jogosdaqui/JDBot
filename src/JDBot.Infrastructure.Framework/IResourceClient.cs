using System.Threading.Tasks;

namespace JDBot.Infrastructure.Framework
{
    public interface IResourceClient
    {
        Task<ImageResource> DownloadImageAsync(string url);
    }
}
