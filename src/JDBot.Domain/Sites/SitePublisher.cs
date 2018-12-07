using System.Threading.Tasks;

namespace JDBot.Domain.Sites
{
    public class SitePublisher
    {
        private readonly ISitePublicationProxy _proxy;

        public SitePublisher(ISitePublicationProxy proxy)
        {
            _proxy = proxy;
        }

        public async Task PublishAsync()
        {
            await _proxy.PublishAsync();
        }

        public async Task<PublicationStatus> GetLatestPublicationStatus()
        {
            return await _proxy.GetPublicationStatusAsync();
        }
    }
}
