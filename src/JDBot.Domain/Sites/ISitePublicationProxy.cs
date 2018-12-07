using System.Threading.Tasks;

namespace JDBot.Domain.Sites
{
    public interface ISitePublicationProxy
    {
        Task PublishAsync();
        Task<PublicationStatus> GetPublicationStatusAsync();
    }
}