﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JDBot.Application;
using JDBot.Domain.Posts;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.IO;
using McMaster.Extensions.CommandLineUtils;

namespace JDBot.ConsoleApp.Commands.Posts
{
    [Command(Name = "grab", Description = "Escreve posts no formato do site a partir de urls de outros posts.")]
    [Subcommand("new", typeof(NewSubcommand))]
    public class GrabSubcommand : PostSubcommandBase
    {
        // Opções para chamada direta, sem arquivo.
        [Option("--url", Description = "A URL do post original")]
        public string Url { get; set; }

        [Option("--title", Description = "O título do post.")]
        public string Title { get; set; }

        [Option("--date", Description = "A data do post")]
        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        [Option("--author", Description = "O nome do autor do posl")]
        public string Author { get; set; }

        // Opção por arquivo.
        [Option("--file", Description = "Arquivo com lista de urls de posts originais")]
        public string File { get; set; }

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            if (await base.OnExecuteAsync(app, console) != 0) return 1;

            Logger.Info("Iniciando...");

            if (!string.IsNullOrEmpty(Url) && !String.IsNullOrEmpty(Jekyll))
            {
                var result = await RunWithArguments();
                OpenPostForEdit(result);
            }
            else if (!String.IsNullOrEmpty(File))
            {
                var results = await RunWithUrlFile();
                OpenPostForEdit(results);
            }
            else 
            {
                app.ShowHelp();
                return 1;
            }

            Logger.Info("Pronto.");
            return 0;
        }

        private async Task<PostInfo> RunWithArguments()
        {
            var postService = new PostService(Jekyll);
            return await postService.WritePostAsync(Url, new PostConfig 
            {
                Title = this.Title,
                Author = this.Author,
                Date = DateTime.Parse(Date),
            });
        }

        private async Task<PostInfo[]> RunWithUrlFile()
        {
            var results = new List<PostInfo>();
            var urlFile = UrlFileParser.Parse(File);
            var postService = new PostService(urlFile.JekyllRootFolder);

            Logger.Info($"Items: {urlFile.Items.Count()}");

            foreach (var item in urlFile.Items)
            {
                results.Add(await postService.WritePostAsync(item.Url, item.Config));
            }

            return results.ToArray();
        }
    }
}
