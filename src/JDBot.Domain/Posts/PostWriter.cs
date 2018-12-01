using System;
using System.IO;
using System.Threading.Tasks;
using JDBot.Infrastructure.Framework;

namespace JDBot.Domain.Posts
{
    public class PostWriter
    {
        private readonly string _jekyllRootFolder;
        private readonly IResourceClient _web;
        private readonly IFileSystem _fs;

        public PostWriter(string jekyllRootFolder, IResourceClient web, IFileSystem fileSystem)
        {
            _jekyllRootFolder = jekyllRootFolder;
            _web = web;
            _fs = fileSystem;
        }

        public async Task WriteAsync(Post post, PostConfig config)
        {
            Logger.Info($"Escrevendo o post {post.Title}...");
            Sanitize(post);

            var content = GetPostContent(post, config);
            var postName = GetPostName(post);
            var date = config.Date ?? post.Date;

            Logger.Debug($"Criando a pasta do post...");
            var postFolder = Path.Combine(_jekyllRootFolder, "_posts", date.Year.ToString());
            _fs.CreateDirectory(postFolder);

            Logger.Debug($"Escrevendo o arquivo do post...");
            var postFileName = Path.Combine(postFolder, $"{date.Year}-{date.Month:00}-{date.Day:00}-{postName}.md");
            _fs.WriteFile(postFileName, content);

            Logger.Debug($"Criando a pasta de imagens do post....");
            var imagesFolder = Path.Combine(_jekyllRootFolder, "assets", date.Year.ToString(), date.Month.ToString("00"), date.Day.ToString("00"), postName);
            _fs.CreateDirectory(imagesFolder);

            Logger.Debug($"Realizando o download das imagens e gravando na pasta...");
            foreach (var screenshot in post.Screenshots)
            {
                var fileName = Path.GetFileName(screenshot);
                var screenshotFileName = Path.Combine(imagesFolder, fileName);

                if (File.Exists(screenshotFileName))
                    Logger.Debug($"Download da imagem {screenshot} já foi realizado anteriormente.");
                else
                {
                    Logger.Debug($"Imagem {screenshot}...");
                    _fs.WriteFile(screenshotFileName, await _web.DownloadImageAsync(screenshot));
                }
            }

            // Grava o logo.
            if (!String.IsNullOrEmpty(post.Logo))
            {
                var logoFileName = Path.Combine(imagesFolder, $"logo{Path.GetExtension(post.Logo)}");

                if (File.Exists(logoFileName))
                    Logger.Debug($"Download do log já foi realizado anteriormente.");
                else
                {
                    Logger.Debug($"Gravando logo...");
                    _fs.WriteFile(logoFileName, await _web.DownloadImageAsync(post.Logo));
                }
            }
        }

        private void Sanitize(Post post)
        {
            if (post.Companies == null)
                post.Companies = new string[0];

            if (post.Screenshots == null)
                post.Screenshots = new string[0];

            if (post.Tags == null)
                post.Tags = new string[0];

            if (post.Videos == null)
                post.Videos = new Video[0];
        }

        private static string GetPostContent(Post post, PostConfig config)
        {
            var content = post.Content.Replace("\n", "\n\n");

            return $@"---
published: true
layout: post
title: '{GetPostTitle(post.Title)}'
author: '{config.Author}'
companies: '{String.Join(" ", post.Companies)}'
categories: {post.Category}
tags: {String.Join(" ", post.Tags)}
---
{content}";
        }

        private static object GetPostTitle(string title)
        {
            return title.Replace("'", "&#39;");
        }

        private static string GetPostName(Post post)
        {
            return post.Title
                       .ToLowerInvariant()
                       .Replace(" ", "-")
                       .Replace(":", "-")
                       .Replace("–", String.Empty)
                       .Replace("--", "-")
                       .Replace("'", String.Empty)
                       .RemoveDiacritics();
                
        }
    }
}
