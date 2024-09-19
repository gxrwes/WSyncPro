// FileSystemService.cs
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WSyncPro.Core.Services
{
    public class FileSystemService : IFileSystemService
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern, bool recursive)
        {
            return Directory.EnumerateFiles(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public long GetFileSize(string filePath)
        {
            return new FileInfo(filePath).Length;
        }
    }
}
