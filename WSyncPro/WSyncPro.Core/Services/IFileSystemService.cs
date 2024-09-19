// IFileSystemService.cs
using System.Collections.Generic;

namespace WSyncPro.Core.Services
{
    public interface IFileSystemService
    {
        bool DirectoryExists(string path);
        IEnumerable<string> EnumerateFiles(string path, string searchPattern, bool recursive);
        long GetFileSize(string filePath);
    }
}
