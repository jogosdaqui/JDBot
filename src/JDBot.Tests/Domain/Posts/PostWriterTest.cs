using NUnit.Framework;
using JDBot.Domain.Posts;
using NSubstitute;
using System.IO;
using System;
using JDBot.Infrastructure.Framework;
using System.Threading.Tasks;

namespace JDBot.Tests.Domain.Posts
{
    [TestFixture]
    public class PostWriterTest
    {
        [Test]
        public async Task Write_Post_Value()
        {
            var jekyllRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jekyll");
            var web = Substitute.For<IResourceClient>();
            var screenshot1 = new ImageResource(new byte[] { 1, 1, 1 }, ".png");
            web.DownloadImageAsync("http://test/screenshot1.png").Returns(screenshot1);

            var screenshot2 = new ImageResource(new byte[] { 2, 2, 2 }, ".png");
            web.DownloadImageAsync("http://test/screenshot2.png").Returns(screenshot2);

            var logo = new ImageResource(new byte[] { 3, 3, 3 }, ".png");
            web.DownloadImageAsync("http://test/screenshot3.png").Returns(logo);

            var fs = Substitute.For<IFileSystem>();

            var target = new PostWriter(jekyllRootFolder, web, fs);
            var post = new Post
            {
                Title = "Test título",
                Date = new DateTime(2018, 11, 28),
                Companies = new string[] { "Test company" },
                Category = PostCategory.Game,
                Tags = new string[] { "test-company", "test-tag" },
                Content = "test content1\ntest content2",
                Logo = "http://test/screenshot3.png",
                Screenshots = new string [] { "http://test/screenshot1.png", "http://test/screenshot2.png" }
            };

            var config = new PostConfig { Author = "Test author" };
            await target.WriteAsync(post, config);

            var expectedPostFolder = Path.Combine(jekyllRootFolder, "_posts", "2018");
            fs.Received().CreateDirectory(expectedPostFolder);

            var expectedPostFilename = Path.Combine(expectedPostFolder, "2018-11-28-test-titulo.md");
            var expectedPostFileContent = @"---
published: true
layout: post
title: 'Test título'
author: 'Test author'
companies: 'Test company'
categories: Game
tags: test-company test-tag
---
test content1

test content2";

            fs.Received().WriteFile(expectedPostFilename, expectedPostFileContent);

            var expectedImagesFolder = Path.Combine(jekyllRootFolder, "assets", "2018", "11", "28", "test-titulo");
            fs.Received().CreateDirectory(expectedImagesFolder);
            fs.Received().WriteFile(Path.Combine(expectedImagesFolder, "screenshot1.png"), screenshot1.Data);
            fs.Received().WriteFile(Path.Combine(expectedImagesFolder, "screenshot2.png"), screenshot2.Data);
            fs.Received().WriteFile(Path.Combine(expectedImagesFolder, "logo.png"), logo.Data);
        }
    }
}
