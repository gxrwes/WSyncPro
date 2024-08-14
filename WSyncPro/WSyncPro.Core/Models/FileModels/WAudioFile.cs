using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Core.Models.FileModels
{
    public class WAudioFile : WFile
    {
        public WAudioFile() 
        { 
            FileType = FileType.Audio; 
        }
        public TimeOnly Length { get; set; }

    }
}
