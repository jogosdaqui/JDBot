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
using SixLabors.ImageSharp.PixelFormats;
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

        public static ImageResource Resize(string filename, byte[] data)
        {
            var result = new ImageResource
            {
                Data = data,
                Extension = Path.GetExtension(filename)
            };

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

                    // Se a imagem têm transparência, então salva com o encoder correspondente,
                    // mas se não tem, então salva com jpg para resultar numa imagem menor (bytes).
                    if (!HasTranparency(image))
                        result.Extension = ".jpg";

                    var encoder = _encoders[result.Extension];

                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, encoder);
                        result.Data = ms.ToArray();
                    }
                }
            }
            catch (NotSupportedException)
            {
                Logger.Warn("Formato de imagem não suportado, não será redimensionada.");
            }

            return result;

        }

        public static bool HasTranparency(Image<Rgba32> image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if ((x == 0 && y == 0) && image[x, y].A == 0)
                        return true;

                }
            }

            return false;
        } 

        public static bool HasTranparency(byte[] data)
        {
            using (var image = Image.Load(data))
            {
                return HasTranparency(image);
            }
        }
    }
}