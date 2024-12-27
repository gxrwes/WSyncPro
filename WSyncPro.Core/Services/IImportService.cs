using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Files;
using WSyncPro.Models.Filter;
using WSyncPro.Models.Import;
using WSyncPro.Models.Jobs;

namespace WSyncPro.Core.Services
{
    public interface IImportService
    {
        public Task<WDirectory> GetFoundFiles(string Path, FilterParams filterParams);
        public Task<string> GenerateDstPathForfile(WFile file, List<ImportPathType> pathBuilder);
        public Task<List<CopyJob>> CreateCopyJobsFromDirectory(WDirectory src, string importDirectory, List<ImportPathType> pathBuilder);
        public Task<bool> RunImport(string srcPath, string dstPath, FilterParams filterParams, List<ImportPathType> pathBuilder);
    }
}
