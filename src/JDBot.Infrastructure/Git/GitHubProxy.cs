using System.Threading.Tasks;
using JDBot.Infrastructure.Framework;
using Octokit;

namespace JDBot.Infrastructure.Git
{
    public class GitHubProxy : IGitHubProxy
    {
        public async Task<SemanticVersioning> GetLatestReleaseAsync()
        {
            var github = new GitHubClient(new ProductHeaderValue("jogosdaqui"));

            SemanticVersioning result = null;
            var tags = await github.Repository.GetAllTags("jogosdaqui", "jogosdaqui.github.io-jekyll");

            foreach (var tag in tags)
            {
                result = SemanticVersioning.Parse(tag.Name);

                if (result != null) break;
            }

            return result;
        }
    }
}
