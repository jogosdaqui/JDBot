using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using JDBot.Application;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Framework;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands.Posts
{
    [Command(Name = "new", Description = "Cria um a estrutura e arquivos para um novo post.")]
    public class NewSubcommand : CommandBase
    {
        [Required]
        [Option("--j", Description = "O caminho da pasta raíz do Jekyll")]
        public string Jekyll { get; set; }

        [Required]
        [Option("--t", Description = "O título do post.")]
        public string Title { get; set; }
      
        [Option("--d", Description = "A data do post")]
        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            await base.OnExecuteAsync(app, console);

            Logger.Info("Iniciando...");

            var service = new PostService(Jekyll);
            var result = await service.WritePostAsync(new Post
            {
                Title = this.Title,
                Date = DateTime.Parse(this.Date)
            });

            Logger.Info($"Abrindo o arquivo do post ...");
            Process.Start(new ProcessStartInfo { FileName = result.FileName, UseShellExecute = true });

            Logger.Info($"Abrindo a pastas de imagens do folder ...");
            Process.Start(new ProcessStartInfo { FileName = result.ImagesFolder, UseShellExecute = true });

            Logger.Info("Pronto.");
            return 0;
        }
    }
}
