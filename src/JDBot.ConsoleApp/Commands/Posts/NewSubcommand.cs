using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JDBot.Application;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Framework;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands.Posts
{
    [Command(Name = "new", Description = "Cria o arquivo e a estrutura de pasta para um novo post.")]
    public class NewSubcommand : PostSubcommandBase
    { 
        [Required]
        [Argument(0, Description = "O título do post.")]
        public string Title { get; set; }
      
        [Option("--date", Description = "A data do post")]
        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            if (await base.OnExecuteAsync(app, console) != 0) return 1;

            Logger.Info("Iniciando...");

            var service = new PostService(Jekyll);
            var result = await service.WritePostAsync(new Post
            {
                Title = this.Title,
                Date = DateTime.Parse(this.Date)
            });

            OpenPostForEdit(result);

            Logger.Info("Pronto.");
            return 0;
        }
    }
}
