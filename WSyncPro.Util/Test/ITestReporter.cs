using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSyncPro.Util.Test.TestModels;

namespace WSyncPro.Util.Test
{
    public interface ITestReporter
    {
        public Task<bool> Report(TestReport report);
        public Task<bool> UpdateReport(TestReport report);
        public Task<string> BuildFinalReport();
    }
}
