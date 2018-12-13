using System;
using System.IO;
using JDBot.Infrastructure.Framework;

namespace JDBot.Infrastructure.IO
{
    public class FileSystem : IFileSystem
    {
        public void ChangeCurrentDirectory(string path)
        {
            Environment.CurrentDirectory = path;
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool ExistsDirectory(string directory)
        {
            return Directory.Exists(directory);
        }

        public bool ExistsFile(string filename)
        {
            return File.Exists(filename);
        }

        public void MoveDirectory(string oldDirectory, string newDirectory)
        {
            var previousDirectory = Path.GetDirectoryName(newDirectory);

            if(!Directory.Exists(previousDirectory))
                Directory.CreateDirectory(previousDirectory);

            Directory.Move(oldDirectory, newDirectory);
        }

        public void MoveFile(string oldFilename, string newFilename)
        {
            File.Move(oldFilename, newFilename);
        }

        public void WriteFile(string filename, string content)
        {
            File.WriteAllText(filename, content);
        }

        public void WriteFile(string filename, byte[] data)
        {
            File.WriteAllBytes(filename, data);
        }
        
    }
}
