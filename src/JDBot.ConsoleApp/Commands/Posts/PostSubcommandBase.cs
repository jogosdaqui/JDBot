using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Framework;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands
{
    public abstract class PostSubcommandBase : SubCommandBase
    {
        [Option("--jekyll", Description = "A pasta raíz do site Jekyll. Pode ser definida diretamente no comando ou através da variável de ambiente JDBOT_JEKYLL_FOLDER")]
        public string Jekyll { get; set; } = Environment.GetEnvironmentVariable("JDBOT_JEKYLL_FOLDER");

        protected async override Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            await base.OnExecuteAsync(app, console);

            if(String.IsNullOrEmpty(Jekyll))
            {
                app.ShowHelp();
                return 1;
            }

            return 0;
        }

        protected void OpenPostForEdit(params PostInfo[] results)
        {
            foreach (var result in results)
            {
                Logger.Info($"Abrindo o arquivo do post {result.FileName}...");
                Process.Start(new ProcessStartInfo { FileName = result.FileName, UseShellExecute = true });

                Logger.Info($"Abrindo a pastas de imagens do folder {result.ImagesFolder}...");
                Process.Start(new ProcessStartInfo { FileName = result.ImagesFolder, UseShellExecute = true });
            }
        }
    }
}
