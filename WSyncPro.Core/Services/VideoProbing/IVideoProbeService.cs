﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Core.Services.VideoProbing
{
    public interface IVideoProbeService
    {
        public Task RunInfoProbe(string folderpath);
    }
}
