using System.Threading.Tasks;
using JDBot.Infrastructure.Framework;

namespace JDBot.Domain.Sites
{
    public class SiteReleaser
    {
        private readonly string _jekyllRepositoryRootFolder;
        private readonly IFileSystem _fs;
        private readonly IGitHubProxy _gitHub;
        private readonly IGitProxy _git;
        private readonly IGitFlowProxy _gitFlow;

        public SiteReleaser(string jekyllRepositoryRootFolder, IFileSystem fs, IGitHubProxy gitHub, IGitProxy git, IGitFlowProxy gitFlow)
        {
            _jekyllRepositoryRootFolder = jekyllRepositoryRootFolder;
            _fs = fs;
            _gitHub = gitHub;
            _git = git;
            _gitFlow = gitFlow;
        }

        public async Task<SemanticVersioning> ReleaseAsync(string message, bool isPatch = false)
        {
            var releaseVersion = await _gitHub.GetLatestReleaseAsync();

            if (isPatch)
                releaseVersion.Patch++;
            else
            {
                releaseVersion.Minor++;
                releaseVersion.Patch = 0;
            }

            _fs.ChangeCurrentDirectory(_jekyllRepositoryRootFolder);
            _gitFlow.StartRelease(releaseVersion);
            _gitFlow.FinishRelease(releaseVersion, message);
            _git.Checkout("master");
            _git.Push();
            _git.PushTags();
            _git.Checkout("develop");
            _git.Push();

            return releaseVersion;
        }
    }
}