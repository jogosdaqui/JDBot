using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JDBot.Application;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.IO;
using JDBot.Infrastructure.Logging;
using Mono.Options;

namespace JDBot.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var urls = new List<string>();
            string file = null;
            string jekyllRootFolder = null;
            var date = DateTime.Now;
            string author = null;
            var verbosity = LogVerbosity.Info;

            var p = new OptionSet() {
                { "u|url=", "A URL do post original", v => urls.Add(v) },
                { "f|file=", "Arquivo com lista de urls de posts originais", v => file = v },
                { "j|jekyll=", "O caminho da pasta raíz do Jekyll", v => jekyllRootFolder = v },
                { "d|date=", "A data do post", v => date = DateTime.Parse(v) },
                { "a|author=", "O nome do autor do post", v => author = v },
                { "v|verbosity=", "Define o nível da verbosidade (debug, info, warn ou error", v => verbosity = Enum.Parse<LogVerbosity>(v) }
            };

            p.Parse(args);

            Logger.Initialize(new ConsoleLogger(), verbosity);
            Logger.Debug("Iniciando...");
          
            if (urls.Count > 0 && !String.IsNullOrEmpty(jekyllRootFolder))
            {
                await RunWithArguments(urls, jekyllRootFolder, date, author);
            }
            else if (!String.IsNullOrEmpty(file))
            {
                await RunWithUrlFile(file);
            }
            else 
            {
                Logger.Error("INVALID ARGUMENTS!");
            }
            Logger.Info("Pronto.");
        }

        private static async Task RunWithArguments(List<string> urls, string jekyllRootFolder, DateTime date, string author)
        {
            Logger.Debug($"Pasta raíz do Jekyll: {jekyllRootFolder}");

            var postService = new PostService(jekyllRootFolder);

            foreach (var url in urls)
            {
                await postService.WritePostAsync(url, new PostConfig { Author = author, Date = date });
            }
        }

        private static async Task RunWithUrlFile(string file)
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
