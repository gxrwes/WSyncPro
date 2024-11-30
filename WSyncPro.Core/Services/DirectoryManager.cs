using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WSyncPro.Models.Files;

namespace WSyncPro.Core.Services
{
    public class DirectoryManager : IDirectoryManagerService
    {
        private readonly Dictionary<Guid, WBaseItem> _cacheById = new();
        private readonly Dictionary<string, WBaseItem> _cacheByName = new(StringComparer.OrdinalIgnoreCase);

        public async Task<WBaseItem> GetItemById(Guid id)
        {
            if (_cacheById.TryGetValue(id, out var item))
            {
                return item;
            }

            throw new KeyNotFoundException($"Item with ID {id} not found.");
        }

        public async Task<WBaseItem> GetItemByName(string name)
        {
            if (_cacheByName.TryGetValue(name, out var item))
            {
                return item;
            }

            throw new KeyNotFoundException($"Item with name {name} not found.");
        }

        public async Task<List<WDirectory>> GetAllRootDirectories()
        {
            var rootDirectories = _cacheById.Values.OfType<WDirectory>().Where(d => string.IsNullOrEmpty(Path.GetDirectoryName(d.Path))).ToList();
            return rootDirectories;
        }

        public async Task<WDirectory> ScanDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found: {path}");
            }

            var directoryInfo = new DirectoryInfo(path);

            var wDirectory = new WDirectory
            {
                Id = Guid.NewGuid(),
                Name = directoryInfo.Name,
                Path = directoryInfo.FullName,
                Created = directoryInfo.CreationTime,
                LastUpdated = directoryInfo.LastWriteTime,
                Items = new List<WBaseItem>()
            };

            foreach (var dir in directoryInfo.GetDirectories())
            {
                var subDirectory = await ScanDirectory(dir.FullName);
                wDirectory.Items.Add(subDirectory);
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                var wFile = new WFile
                {
                    Id = Guid.NewGuid(),
                    Name = file.Name,
                    Path = file.FullName,
                    FileExtension = file.Extension,
                    FileSize = (int)file.Length,
                    Created = file.CreationTime,
                    LastUpdated = file.LastWriteTime
                };

                wDirectory.Items.Add(wFile);
                CacheItem(wFile);
            }

            CacheItem(wDirectory);

            return wDirectory;
        }

        private void CacheItem(WBaseItem item)
        {
            _cacheById[item.Id] = item;
            _cacheByName[item.Name] = item;
        }
    }
}
