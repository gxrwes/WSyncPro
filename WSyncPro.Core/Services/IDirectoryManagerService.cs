using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Files;


namespace WSyncPro.Core.Services
{
    /*
     * For managing directories and files
     * Just gathers information
     * holds scannes in cache
     */
    public interface IDirectoryManagerService
    {
        // manages client cached directories
        public Task<WBaseItem> GetItemById(Guid id);
        public Task<WBaseItem> GetItemByName(string name);
        public Task<List<WDirectory>> GetAllRootDirectories();

        // Scans the path given, either relative or absolute, and builds the according directory items, adds items to cache for quicker access
        public Task<WDirectory> ScanDirectory(string path);
    }
}
