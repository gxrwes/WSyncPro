using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Core.Models.FileModels
{
    public class WImageFile : WFile
    {
        Resolution Resolution { get; set; }
        public WImageFile() 
        {
            FileType = FileType.Image;

        }
    }

}
