﻿using System;
using System.Collections.Generic;
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

        public async Task WriteAsync(Post post)
        {
            await WriteAsync(post, PostConfig.Empty);
        }

        public async Task<PostInfo> WriteAsync(Post post, PostConfig config)
        {
            Logger.Info($"Escrevendo o post {post.Title}...");
            Sanitize(post);

            var result = new PostInfo(post.Title, post.Date);

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
                Logger.Debug($"Screenshot {screenshot}");
                var image = await _web.DownloadImageAsync(screenshot);

                if (image.Data.Length >= config.IgnoreImagesLowerThanBytes)
                {
                    var fileName = Path.Combine(imagesFolder, $"{Path.GetFileNameWithoutExtension(screenshot)}{image.Extension}");

                    Logger.Debug($"Gravando screenshot {fileName}...");
                    _fs.WriteFile(fileName, image.Data);
                }
                else
                    Logger.Warn("Não será gravada, pois é menor que o tamanho mínimo esperado.");
            }

            // Grava o logo.
            if (!String.IsNullOrEmpty(post.Logo))
            {
                var image = await _web.DownloadImageAsync(post.Logo);
                var fileName = Path.Combine(imagesFolder, $"logo{image.Extension}");

                Logger.Debug($"Gravando logo {fileName}...");
                _fs.WriteFile(fileName, image.Data);
            }

            return result;
        }

        public async Task<PostInfo> RenameAsync(PostInfo oldPost, PostInfo newPost)
        {
            oldPost = new PostInfo(_jekyllRootFolder, oldPost.Title, oldPost.Date);
            newPost = new PostInfo(_jekyllRootFolder, newPost.Title, newPost.Date);

            if (!_fs.ExistsFile(oldPost.FileName))
                throw new ArgumentException(nameof(oldPost), $"O arquivo antigo do post não existe: {oldPost.FileName}");

            if (!_fs.ExistsDirectory(oldPost.ImagesFolder))
                throw new ArgumentException(nameof(oldPost), $"A pasta de imagens antiga do post não existe: {oldPost.ImagesFolder}");

            if (_fs.ExistsFile(newPost.FileName))
                throw new ArgumentException(nameof(newPost), $"O arquivo do novo post já existe: {newPost.FileName}");

            if (_fs.ExistsDirectory(newPost.ImagesFolder))
                throw new ArgumentException(nameof(newPost), $"A pasta de imagens do novo post já existe: {newPost.ImagesFolder}");

            _fs.MoveFile(oldPost.FileName, newPost.FileName);
            _fs.MoveDirectory(oldPost.ImagesFolder, newPost.ImagesFolder);
        
            return await Task.Run(() => newPost);
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
            var content = String.IsNullOrEmpty(post.Content) ? String.Empty : post.Content.Replace("\n", "\n\n");

            content = AddVideos(post.Videos, content);

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

        private static string AddVideos(IEnumerable<Video> videos, string content)
        {
            foreach(var video in videos)
                content += $"\n\n{{% {video.Kind.ToString().ToLowerInvariant()} {video.Id} %}}";

            return content;
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
