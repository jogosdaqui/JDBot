using System.Threading.Tasks;

namespace JDBot.Domain.Sites
{
    public interface ISitePublisher
    {
        Task PublishAsync();
        Task<PublicationStatus> GetPublicationStatusAsync();
    }
}
