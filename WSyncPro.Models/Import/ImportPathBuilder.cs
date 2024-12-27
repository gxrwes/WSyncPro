using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Files;

namespace WSyncPro.Models.Import
{
    public class ImportPathBuilder
    {
        public string GetBuiltPath(WFile originalFile, List<ImportPathType> pathBuilder, int currentCounter)
        {
            if (originalFile == null) throw new ArgumentNullException(nameof(originalFile));
            var filename = originalFile.Name;

            var builtPath = new StringBuilder();

            foreach (var pathType in pathBuilder)
            {
                builtPath.Append(GetValueForString(pathType, currentCounter, originalFile.Id.ToString(), filename));
                builtPath.Append("/"); // Assuming paths are separated by '/'
            }

            // Remove the trailing slash
            if (builtPath.Length > 0)
                builtPath.Length--;

            return builtPath.ToString();
        }

        private string GetValueForString(ImportPathType type, int currentCounter, string id = "", string fileName = "")
        {
            switch (type)
            {
                case ImportPathType.Video:
                case ImportPathType.Audio:
                case ImportPathType.Photo:
                    return type.ToString();
                case ImportPathType.DateFull:
                    return DateTime.Now.ToString("yyyy-MM-dd");
                case ImportPathType.DateMonth:
                    return DateTime.Now.Month.ToString("D2"); // Ensure 2-digit month format
                case ImportPathType.DateYear:
                    return DateTime.Now.Year.ToString();
                case ImportPathType.DateYearMonth:
                    return DateTime.Now.ToString("yyyy-MM"); // Format as YYYY-MM
                case ImportPathType.Counter:
                    return $"[{currentCounter}]";
                case ImportPathType.ID:
                    return !string.IsNullOrEmpty(id) ? id : "[No ID Provided]";
                case ImportPathType.FileName:
                    return !string.IsNullOrEmpty(fileName) ? fileName : "[No FileName Provided]";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unhandled ImportPathType");
            }
        }
    }
}
