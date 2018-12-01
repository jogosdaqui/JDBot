using System;
using System.Collections.Generic;
using System.IO;
using JDBot.Infrastructure.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace JDBot.Infrastructure.Drawing
{
    public static class ImageEditor
    {
        private static readonly Dictionary<string, IImageEncoder> _encoders = new Dictionary<string, IImageEncoder>()
        {
            { ".bmp", new BmpEncoder() },
            { ".gif", new GifEncoder() },
            { ".jpg", new JpegEncoder { Quality = 75 } },
            { ".png", new PngEncoder { CompressionLevel = 10 } }
        };

        public static byte[] Resize(string filename, byte[] data)
        {
            try
            {
                using (var image = Image.Load(data))
                {
                    if (image.Width > 1280)
                    {
                        Logger.Debug("Redimensionando a imagem...");

                        var options = new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(1280, 720)
                        };

                        image.Mutate(x => x.Resize(options));
                    }

                    Logger.Debug("Salvando a imagem ...");
                    var encoder = _encoders[Path.GetExtension(filename)];

                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, encoder);
                        return ms.ToArray();
                    }

                }
            }
            catch (NotSupportedException)
            {
                Logger.Warn("Formato de imagem não suportado, não será redimensionada.");
            }

            return data;

        }

        public static bool HasTranparency(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}