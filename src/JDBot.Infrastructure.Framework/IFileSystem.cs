namespace JDBot.Infrastructure.Framework
{
    public interface IFileSystem
    {
        void WriteFile(string filename, string content);
        void WriteFile(string filename, byte[] data);
        void CreateDirectory(string path);
        void ChangeCurrentDirectory(string path);
        void MoveFile(string oldFilename, string newFilename);
        void MoveDirectory(string oldDirectory, string newDirectory);
        bool ExistsFile(string filename);
        bool ExistsDirectory(string directory);
    }
}
