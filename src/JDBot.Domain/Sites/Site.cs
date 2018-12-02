using System;
using System.Threading.Tasks;

namespace JDBot.Domain.Sites
{
    public class Site
    {
        private readonly ISitePublisher _publisher;

        public Site(ISitePublisher publisher)
        {
            _publisher = publisher;
        }

        public string Url { get; }

        public async Task PublishAsync()
        {
           await _publisher.PublishAsync();
        }

        public async Task<PublicationStatus> GetLatestPublicationStatus()
        {
            return await _publisher.GetPublicationStatusAsync();
        }
    }
}
