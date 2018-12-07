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
