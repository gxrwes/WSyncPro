using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Core.Test.TestModels;

namespace WSyncPro.Core.Test
{
    public interface ITestReporter
    {
        public Task<bool> Report(TestReport report);
        public Task<bool> UpdateReport(TestReport report);
        public Task<string> BuildFinalReport();
    }
}
