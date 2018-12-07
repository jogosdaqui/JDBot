using System.Threading.Tasks;

namespace JDBot.Infrastructure.Framework
{
    public interface IGitHubProxy
    {
        Task<SemanticVersioning> GetLatestReleaseAsync();
    }
}
