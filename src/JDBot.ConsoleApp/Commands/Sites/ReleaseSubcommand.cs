using System;
using System.Threading.Tasks;
using JDBot.Domain.Sites;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.Git;
using JDBot.Infrastructure.IO;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands.Sites
{
    [Command(Name = "release", Description = "Libera uma nova versão do site no GitHub a partir do que existe no develop.")]
    public class ReleaseSubcommand : SubCommandBase
    {
        [Option("--repo-folder", Description = "A pasta raiz do repositório do jogodaqui.github.io-jekyll. Pode ser definida diretamente no comando ou através da variável de ambiente JDBOT_REPO_FOLDER")]
        public string RepoFolder { get; set; } = Environment.GetEnvironmentVariable("JDBOT_REPO_FOLDER");

      
        [Option("--message", Description = "A mensagem que será utilizada na tag/release.")]
        public string Message { get; set; }

        [Option("--patch", Description = "Se um patch, uma correção da última release.")]
        public bool Patch { get; set; }

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            await base.OnExecuteAsync(app, console);

            if (String.IsNullOrEmpty(RepoFolder))
            {
                app.ShowHelp();
                return 1;
            }

            var fs = new FileSystem();
            var gitHub = new GitHubProxy();
            var git = new GitProxy();
            var gitFlow = new GitFlowProxy();
            var releaser = new SiteReleaser(RepoFolder, fs, gitHub, git, gitFlow);

            var version = await releaser.ReleaseAsync(Message, Patch);

            Logger.Info($"Versão: {version} liberada no master do GitHub.");
           
            return 0;
        }
    }
}
