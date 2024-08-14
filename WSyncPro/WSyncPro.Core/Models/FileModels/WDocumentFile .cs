using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Core.Models.FileModels
{
    public class WDocumentFile : WFile
    {
        public int PageCount { get; set; }
        public string Format { get; set; } // e.g., PDF, DOCX, TXT
        public WDocumentFile()
        {
            FileType = FileType.Document;
        }
    }
}
