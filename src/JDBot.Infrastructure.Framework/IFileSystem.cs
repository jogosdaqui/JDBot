using System;

namespace JDBot.Infrastructure.Framework
{
    public interface IFileSystem
    {
        void WriteFile(string filename, string content);
        void WriteFile(string filename, byte[] data);
        void CreateDirectory(string path);
        void ChangeCurrentDirectory(string path);
    }
}
