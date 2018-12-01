using System.Net;
using System.Threading.Tasks;
using JDBot.Infrastructure.Drawing;
using JDBot.Infrastructure.Framework;

namespace JDBot.Infrastructure.Net
{
    public class WebResourceClient : IResourceClient
    {
        public async Task<ImageResource> DownloadImageAsync(string url)
        {
            using (var client = new WebClient())
            {
                return await Task.Run(() =>
                {
                    var data = client.DownloadData(url);
                    return ImageEditor.Resize(url, data);
                });
            }
        }
    }
}
