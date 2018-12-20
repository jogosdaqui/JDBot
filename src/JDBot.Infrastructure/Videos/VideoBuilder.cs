using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JDBot.Infrastructure.Framework;
using JDBot.Infrastructure.IO;

namespace JDBot.Infrastructure.Videos
{
    public class VideoBuilder : IVideoBuilder
    {
        private readonly VideoPart _coverImage;
        private List<VideoPart> _parts = new List<VideoPart>();
        private TimeSpan _start = TimeSpan.Zero;

        public VideoBuilder()
        {
            _coverImage = new VideoPart
            {
                Image = new ImageResource(FileSystem.GetOutputPath("Videos", "video-cover.png")),
                Duration = TimeSpan.FromSeconds(3)
            };
        }

        public IVideoBuilder AddDescription(string text, TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public IVideoBuilder AddImage(ImageResource image, TimeSpan duration)
        {
            _parts.Add(new VideoPart
            {
                Image = image,
                Start = _start,
                Duration = duration
            });

            _start += duration;

            return this;
        }

        public IVideoBuilder AddTitle(string text,  TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public string Build()
        {
            // https://ffmpeg.org/ffmpeg-formats.html#concat-1

            var parts = _parts.OrderBy(p => p.Start);
            var imageParts = parts.Where(p => p.Image != null).ToArray();
            var imagesFilename = new List<string>();
            var inputFilename = FileSystem.GetOutputPath("input.txt");
            var imagesArguments = new List<string>();

            string filename = null;

            using (var inputFileStream = new StreamWriter(inputFilename))
            {
                VideoPart imagePart = null;
                 int number = 0;

                filename = AddImage(inputFileStream, ++number, _coverImage);
                imagesArguments.Add($"-t {_coverImage.Duration.TotalSeconds:N0} -i {filename}");

                foreach (var part in imageParts)
                {
                    imagePart = part;
                    filename = AddImage(inputFileStream, ++number, part);

                    imagesArguments.Add($"-t {part.Duration.TotalSeconds:N0} -i {filename}");
                }

                filename = AddImage(inputFileStream, ++number, _coverImage);
                imagesArguments.Add($"-t {_coverImage.Duration.TotalSeconds:N0} -i {filename}");

                // Due to a quirk, the last image has to be specified twice - the 2nd time without any duration directive.
                // https://trac.ffmpeg.org/wiki/Slideshow
                AddImage(inputFileStream, ++number, imagePart, false);
            }

            var outputFilename = FileSystem.GetOutputPath("output.mp4");
            var imagesLength = imagesArguments.Count();
            Run($"-f image2 -framerate 1/5 -i input_%04d.png -c:v libx264 -pix_fmt yuv420p -vf scale=1280:720:force_original_aspect_ratio=decrease {outputFilename}");

            return outputFilename;
        }

        private string AddImage(StreamWriter inputFileStream, int number, VideoPart part, bool writeFileImageFile = true)
        {
            var filename = $"input_{number:0000}.png";
            inputFileStream.WriteLine($"file '{filename}'");

            if (writeFileImageFile)
            {
                inputFileStream.WriteLine($"duration {part.Duration.TotalSeconds:N0}");
                File.WriteAllBytes(filename, part.Image.Data);
            }

            return filename;
        }

        private void Run(string arguments)
        {
            var info = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments =$"-y {arguments}",
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            var ps = Process.Start(info);
            ps.WaitForExit();
           
           var error = ps.StandardError.ReadToEnd();

            if (ps.ExitCode != 0 || error.Contains("invalid"))
            {
                if (String.IsNullOrEmpty(error))
                    error = ps.StandardOutput.ReadToEnd();

                throw new InvalidOperationException($"ffmpeg {arguments}\n{error}");
            }

        }
    }
}
