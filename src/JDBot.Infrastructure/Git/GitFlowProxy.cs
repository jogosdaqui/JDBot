using JDBot.Infrastructure.Framework;

namespace JDBot.Infrastructure.Git
{
    public class GitFlowProxy : IGitFlowProxy
    {
        public void Init()
        {
            Run("init --defaults");
        }

        public void StartRelease(SemanticVersioning version)
        {
            Run($"release start {version}");
        }

        public void FinishRelease(SemanticVersioning version, string message)
        {
            Run($"release finish {version} --message \"{message}\"");
        }

        private void Run(string command)
        {
            GitProxy.Run($"flow {command}");
        }
    }
}
