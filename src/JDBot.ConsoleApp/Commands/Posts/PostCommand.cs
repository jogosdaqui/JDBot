using System;
using System.Linq;
using System.Threading.Tasks;
using JDBot.Application;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.IO;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands.Posts
{
    [Command(Name = "post", Description = "Escreve posts no formato do site a partir de urls de outros posts.")]
    [Subcommand("new", typeof(NewSubcommand))]
    public class PostCommand : CommandBase
    {
        // Opções para chamada direta, sem arquivo.
        [Option("--u", Description = "A URL do post original")]
        public string Url { get; set; }

        [Option("--j", Description = "O caminho da pasta raíz do Jekyll")]
        public string Jekyll { get; set; }

        [Option("--d", Description = "A data do post")]
        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Option("--a", Description = "O nome do autor do posl")]
        public string Author { get; set; }

        // Opção por arquivo.
        [Option("--f", Description = "Arquivo com lista de urls de posts originais")]
        public string File { get; set; }

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            await base.OnExecuteAsync(app, console);

            Logger.Info("Iniciando...");

            if (!string.IsNullOrEmpty(Url) && !String.IsNullOrEmpty(Jekyll))
            {
                await RunWithArguments(Url, Jekyll, DateTime.Parse(Date), Author);
            }
            else if (!String.IsNullOrEmpty(File))
            {
                await RunWithUrlFile(File);
            }
            else 
            {
                app.ShowHelp();
                return 1;
            }

            Logger.Info("Pronto.");
            return 0;
        }

        private async Task RunWithArguments(string url, string jekyllRootFolder, DateTime date, string author)
        {
            Logger.Debug($"Pasta raíz do Jekyll: {jekyllRootFolder}");

            var postService = new PostService(jekyllRootFolder);
            await postService.WritePostAsync(url, new PostConfig { Author = author, Date = date });
        }

        private async Task RunWithUrlFile(string file)
        {
            var urlFile = UrlFileParser.Parse(file);
            Logger.Debug($"Pasta raíz do Jekyll: {urlFile.JekyllRootFolder}");

            var postService = new PostService(urlFile.JekyllRootFolder);

            Logger.Info($"Items: {urlFile.Items.Count()}");

            foreach (var item in urlFile.Items)
            {
                await postService.WritePostAsync(item.Url, item.Config);
            }
        }
    }
}
