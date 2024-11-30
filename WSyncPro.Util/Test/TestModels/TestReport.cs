using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Util.Test.TestModels
{
    public class TestReport
    {
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
        public TestStatus Status { get; set; }
        public string TestName { get; set; }
        public string Message { get; set; }
        public string Log { get; set; }
        public string Parent { get; set; }
        public List<string> Tags { get; set; }
    }

}
