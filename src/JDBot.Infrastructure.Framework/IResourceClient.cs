using System;
using System.Threading.Tasks;

namespace JDBot.Infrastructure.Framework
{
    public interface IResourceClient
    {
        Task<byte[]> DownloadImageAsync(string url);
    }
}
