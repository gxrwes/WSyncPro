using System;
using WSyncPro.Core.Models;
using WSyncPro.Core.Models.FileModels;
using WSyncPro.Core.Services.ArchiveServices;
using WSyncPro.Core.Services.ScanService;

namespace WSyncPro.Core.Services
{
    public class ImportService
    {
        private readonly CopyService _copyService;
        private readonly DirectoryScanner _directoryScanner;

        public ImportService(CopyService copyService, DirectoryScanner directoryScanner)
        {
            _copyService = copyService ?? throw new ArgumentNullException(nameof(copyService));
            _directoryScanner = directoryScanner ?? throw new ArgumentNullException(nameof(directoryScanner));
        }

        public void ExecuteImport(Job job, UsbDeviceInfo usbDevice)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            if (usbDevice == null)
                throw new ArgumentNullException(nameof(usbDevice));

            // Scan the USB device to get the directory structure
            var sourceDirectory = _directoryScanner.ScanDirectory(usbDevice.Name, job.TargetedFileTypes, job.FilterStrings);

            // Generate the target path using the job's archive options and DirectoryStructureReplacer
            string targetBasePath = job.TargetDirectory;
            string groupedPath = ArchiveFunctionReplacer.DirectoryStructureReplacer(job.ArchiveOptions.GroupingPattern, job, sourceDirectory);

            // Combine the base target path with the generated grouped path
            string fullTargetPath = System.IO.Path.Combine(targetBasePath, groupedPath);

            // Perform the file import using the copy service
            _copyService.CopyFiles(sourceDirectory, job, FileOverwriteOptions.ALWAYS, keepDirectoryStructure: true);
        }
    }
}
