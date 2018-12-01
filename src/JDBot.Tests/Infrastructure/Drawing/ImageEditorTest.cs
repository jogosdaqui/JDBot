using System;
using System.IO;
using JDBot.Infrastructure.Drawing;
using NUnit.Framework;

namespace JDBot.Tests.Infrastructure.Drawing
{
    [TestFixture]
    public class ImageEditorTest
    {
        private static string _hasTransparencyImageFileName;
        private static string _hasNoTransparencyImageFileName;

        [OneTimeSetUp]
        public void SetupFixture()
        {
           var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Infrastructure", "Drawing");
            _hasTransparencyImageFileName = Path.Combine(root, "HasTransparencyImage.png");
            _hasNoTransparencyImageFileName = Path.Combine(root, "HasNoTransparencyImage.jpg");
        }

        [Test]
        public void HasTransparency_ImageHasNoTransparentBit_False()
        {
            var data = File.ReadAllBytes(_hasNoTransparencyImageFileName);
            var actual = ImageEditor.HasTranparency(data);
            Assert.IsFalse(actual);
        }

        [Test]
        public void HasTransparency_ImageHasTransparentBit_True()
        {
            var data = File.ReadAllBytes(_hasTransparencyImageFileName);
            var actual = ImageEditor.HasTranparency(data);
            Assert.IsTrue(actual);
        }
    }
}
