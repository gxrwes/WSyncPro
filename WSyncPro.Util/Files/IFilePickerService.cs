using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Util.Files
{
    public interface IFilePickerService
    {
        Task<string> PickFileToOpenAsync();
        Task<string> PickFileToSaveAsync(string suggestedFileName);
    }
}
