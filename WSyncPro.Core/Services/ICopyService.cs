using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Models.Jobs;

namespace WSyncPro.Core.Services
{
    public interface ICopyService
    {
        public Task CopyFile(CopyJob copyJob);
        public Task MoveFile(CopyJob moveJob);
    }
}
