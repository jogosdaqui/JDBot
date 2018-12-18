using NUnit.Framework;
using JDBot.Domain.Posts;
using NSubstitute;
using System.IO;
using System;
using JDBot.Infrastructure.Framework;
using System.Threading.Tasks;
using System.Linq;

namespace JDBot.Tests.Domain.Posts
{
    [TestFixture]
    public class PostWriterTest
    {
        private string _jekyllRootFolder;
        private IResourceClient _web;
        private IFileSystem _fs;
        private PostWriter _target;

        [SetUp]
        public void Setup()
        {
            _jekyllRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jekyll");
            _web = Substitute.For<IResourceClient>();
            _fs = Substitute.For<IFileSystem>();
            _target = new PostWriter(_jekyllRootFolder, _web, _fs);
        }


        [Test]
        public async Task WriteAsync_Post_Value()
        {
            var screenshot1 = new ImageResource(new byte[] { 1, 1, 1 }, ".png");
            _web.DownloadImageAsync("http://test/screenshot1.png").Returns(screenshot1);

            var screenshot2 = new ImageResource(new byte[] { 2, 2, 2 }, ".png");
            _web.DownloadImageAsync("http://test/screenshot2.png").Returns(screenshot2);

            var screenshot3 = new ImageResource(new byte[] { 2, 2 }, ".png");
            _web.DownloadImageAsync("http://test/screenshot3.png").Returns(screenshot3);

            var logo = new ImageResource(new byte[] { 3, 3, 3 }, ".png");
            _web.DownloadImageAsync("http://test/screenshot3.png").Returns(logo);

            var post = new Post
            {
                Title = "Test título",
                Date = new DateTime(2018, 11, 28),
                Companies = new string[] { "Test company" },
                Category = PostCategory.Game,
                Tags = new string[] { "test-company", "test-tag" },
                Content = "test content1\ntest content2",
                Logo = "http://test/screenshot3.png",
                Screenshots = new string[] { "http://test/screenshot1.png", "http://test/screenshot2.png", "http://test/screenshot3.png" }
            };

            var config = new PostConfig { Author = "Test author", IgnoreImagesLowerThanBytes = 3 };
            await _target.WriteAsync(post, config);

            var expectedName = "test-titulo";
            AssertFolder(post);
            AssertContent(
                post,
                expectedName,
                "Test título",
                "Test author",
                "Test company",
                PostCategory.Game,
                "test-company test-tag",
                @"test content1

test content2");
            AssertImagesFolder(post, expectedName);
            AssertScreenshots(post, expectedName, screenshot1, screenshot2);
            AssertLogo(post, expectedName, logo);
        }

        [Test]
        public async Task WritAsync_PostOnlyWithTitleAndDate_Written()
        {
            var post = new Post
            {
                Title = "Test título",
                Date = new DateTime(2018, 11, 28)
            };

            await _target.WriteAsync(post);

            var expectedName = "test-titulo";

            AssertFolder(post);
            AssertContent(
                post,
                expectedName,
                "Test título",
                "",
                "",
                PostCategory.News,
                "",
                "");
            AssertImagesFolder(post, expectedName);
        }

        [Test]
        public void RenameAsync_OldPostFilenameDoesNotExists_Exception()
        {
            var oldPost = new PostInfo("old-post", new DateTime(2018, 12, 8));
            var newPost = new PostInfo("new-post", new DateTime(2019, 11, 7));

            _fs.ExistsFile(oldPost.FileName).Returns(false);

            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _target.RenameAsync(oldPost, newPost);
            });
        }

        [Test]
        public void RenameAsync_OldPostImagesFolderDoesNotExists_Exception()
        {
            var oldPost = new PostInfo("old-post", new DateTime(2018, 12, 8));
            var newPost = new PostInfo("new-post", new DateTime(2019, 11, 7));

            _fs.ExistsFile(oldPost.FileName).Returns(true);
            _fs.ExistsDirectory(oldPost.ImagesFolder).Returns(false);

            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _target.RenameAsync(oldPost, newPost);
            });
        }

        [Test]
        public void RenameAsync_NewPostFilenameAlreadExists_Exception()
        {
            var oldPost = new PostInfo("old-post", new DateTime(2018, 12, 8));
            var newPost = new PostInfo("new-post", new DateTime(2019, 11, 7));

            _fs.ExistsFile(oldPost.FileName).Returns(true);
            _fs.ExistsDirectory(oldPost.ImagesFolder).Returns(true);
            _fs.ExistsFile(newPost.FileName).Returns(true);

            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _target.RenameAsync(oldPost, newPost);
            });
        }

        [Test]
        public void RenameAsync_NewPostImagesFolderAlreadExists_Exception()
        {
            var oldPost = new PostInfo("old-post", new DateTime(2018, 12, 8));
            var newPost = new PostInfo("new-post", new DateTime(2019, 11, 7));

            _fs.ExistsFile(oldPost.FileName).Returns(true);
            _fs.ExistsDirectory(oldPost.ImagesFolder).Returns(true);
            _fs.ExistsFile(newPost.FileName).Returns(false);
            _fs.ExistsDirectory(newPost.ImagesFolder).Returns(true);

            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _target.RenameAsync(oldPost, newPost);
            });
        }

        [Test]
        public async Task RenameAsync_OldPostInfoAndNewPostInfo_Renamed()
        {
            var oldPost = new PostInfo("old-post", new DateTime(2018, 12, 8));
            var newPost = new PostInfo("new-post", new DateTime(2019, 11, 7));

            _fs.ExistsFile(Path.Combine(_jekyllRootFolder, oldPost.FileName)).Returns(true);
            _fs.ExistsDirectory(Path.Combine(_jekyllRootFolder, oldPost.ImagesFolder)).Returns(true);
            _fs.ExistsFile(Path.Combine(_jekyllRootFolder, newPost.FileName)).Returns(false);
            _fs.ExistsDirectory(Path.Combine(_jekyllRootFolder, newPost.ImagesFolder)).Returns(false);
           
            var actual = await _target.RenameAsync(oldPost, newPost);

            var expected = new PostInfo(_jekyllRootFolder, newPost.Title, newPost.Date);
            Assert.AreEqual(expected.FileName, actual.FileName);
            Assert.AreEqual(expected.ImagesFolder, actual.ImagesFolder);

            _fs.Received().MoveFile(Path.Combine(_jekyllRootFolder, oldPost.FileName), Path.Combine(_jekyllRootFolder, newPost.FileName));
            _fs.Received().MoveDirectory(Path.Combine(_jekyllRootFolder, oldPost.ImagesFolder), Path.Combine(_jekyllRootFolder, newPost.ImagesFolder));
        }

        #region Helpers
        private void AssertFolder(Post post)
        {
            string expectedPostFolder = GetExpectedPostFolder(post);
            _fs.Received().CreateDirectory(expectedPostFolder);
        }

        private void AssertContent(Post post, string expectedName, string expectedTitle, string expectedAuthor, string expectedCompany, PostCategory expectedCategory, string expectedTags, string expectedContent)
        {
            var expectedPostFolder = GetExpectedPostFolder(post);
            var expectedPostFilename = Path.Combine(expectedPostFolder, $"{post.Date:yyyy-MM-dd}-{expectedName}.md");
            var expectedPostFileContent = $@"---
published: true
layout: post
title: '{expectedTitle}'
date: '{post.Date:yyyy-MM-dd HH:mm}'
author: '{expectedAuthor}'
companies: '{expectedCompany}'
categories: {expectedCategory}
tags: {expectedTags}
---
{expectedContent}";

            _fs.ReceivedWithAnyArgs().WriteFile(expectedPostFilename, expectedPostFileContent);
        }

        private void AssertImagesFolder(Post post, string expectedName)
        {
            var expectedImagesFolder = GetExpectedImagesFolder(post, expectedName);
            _fs.Received().CreateDirectory(expectedImagesFolder);
        }

        private void AssertScreenshots(Post post, string expectedName, params ImageResource[] screenshots)
        {
            var expectedImagesFolder = GetExpectedImagesFolder(post, expectedName);
            var postScreenshots = post.Screenshots.ToArray();

            for (int i = 0; i < screenshots.Length; i++)
            {
                _fs.Received().WriteFile(Path.Combine(expectedImagesFolder, Path.GetFileName(postScreenshots[i])), screenshots[i].Data);
            }
        }

        private void AssertLogo(Post post, string expectedName, ImageResource logo)
        {
            var expectedImagesFolder = GetExpectedImagesFolder(post, expectedName);
            _fs.Received().WriteFile(Path.Combine(expectedImagesFolder, "logo.png"), logo.Data);
        }

        private string GetExpectedPostFolder(Post post)
        {
            return Path.GetDirectoryName(PostInfo.From(post, _jekyllRootFolder).FileName);
        }

        private string GetExpectedImagesFolder(Post post, string expectedName)
        {
            var root = Path.GetDirectoryName(PostInfo.From(post, _jekyllRootFolder).ImagesFolder);
            return Path.Combine(root, expectedName);
        }
        #endregion
    }
}