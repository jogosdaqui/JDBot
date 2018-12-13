using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JDBot.Application;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Framework;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands.Posts
{
    [Command(Name = "rename", Description = "Renomeia um post mudando sua estrutura de pasta e arquivos.")]
    public class RenameSubcommand : PostSubcommandBase
    { 
        [Required]
        [Option("--old-title", Description = "O título antigo do post.")]
        public string OldTitle { get; set; }
      
        [Option("--old-date", Description = "A data antiga do post")]
        public string OldDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Option("--new-title", Description = "O título novo do post.")]
        public string NewTitle { get; set; }

        [Option("--new-date", Description = "A data nova do post")]
        public string NewDate { get; set; }

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            if (await base.OnExecuteAsync(app, console) != 0) return 1;

            Logger.Info("Iniciando...");

            NewTitle = NewTitle ?? OldTitle;
            NewDate = NewDate ?? OldDate;

            var service = new PostService(Jekyll);
            var oldPost = new PostInfo(OldTitle, DateTime.Parse(OldDate));
            var newPost = new PostInfo(NewTitle, DateTime.Parse(NewDate));
            var result = await service.RenamePostAsync(oldPost, newPost);

            OpenPostForEdit(result);

            Logger.Info("Pronto.");
            return 0;
        }
    }
}
