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

            //Run($"-f concat -safe 0 -i {inputFilename} -vsync vfr -pix_fmt yuv420p {outputFilename}");
            var imagesLength = imagesArguments.Count();
            //Run($"-loop 1 {String.Join(" ", imagesArguments.ToArray())}  -f lavfi -t 3 -i anullsrc -filter_complex \"[{imagesLength}:a]asplit[i][e];[0] fade=out:st=0:d=3[0f];[1] fade=in:st=0:d=3[1f];[0f] [i] [{imagesLength}:v] [{imagesLength}:a] [1f] [e] concat=n={imagesLength}:v=1:a=1[v] [a]\" -map [v] -map [a] {outputFilename}");
            // http://hamelot.io/visualization/using-ffmpeg-to-convert-a-set-of-images-into-a-video/
            Run($"-r 60 -f {filename} -s 1920x1080 -i input_%04d.png -vcodec libx264 -crf 25 -pix_fmt yuv420p test.mp4");

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
