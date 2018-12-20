using System;
using System.IO;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.IO;
using JDBot.Infrastructure.Videos;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Videos
{
    [TestFixture]
    public class VideoBuilderTest
    {
        private ImageResource _sampleImage1;
        private ImageResource _sampleImage2;
        private ImageResource _sampleImage3;
        private ImageResource _sampleImage4;

        [OneTimeSetUp]
        public void Setup()
        {
            var root = FileSystem.GetOutputPath("Infrastructure", "Videos");
            _sampleImage1 = new ImageResource(Path.Combine(root, "sample-image-1.png"));
            _sampleImage2 = new ImageResource(Path.Combine(root, "sample-image-2.png"));
            _sampleImage3 = new ImageResource(Path.Combine(root, "sample-image-3.png"));
            _sampleImage4 = new ImageResource(File.ReadAllBytes(Path.Combine(root, "sample-image-4.jpg")));
        }

        [Test]
        public void Build_OnlyValidImages_VideoWithOnlyImages()
        {
            var target = new VideoBuilder();
            var actual = target
                .AddImage(_sampleImage1, 1)
                .AddImage(_sampleImage2, 2)
                .AddImage(_sampleImage3, 3)
                .AddImage(_sampleImage4, 4)
                .Build();

            FileAssert.Exists(actual);
        }
    }
}
