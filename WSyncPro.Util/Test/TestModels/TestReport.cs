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
        public TestStatus Status { get; set; } = TestStatus.Skipped;
        public string TestName { get; set; } = "Notset";
        public string Message { get; set; } = string.Empty;
        public string Log { get; set; } = String.Empty;
        public string TestStepname { get; set; } = string.Empty ;
        public List<string> Tags { get; set; } = new List<string>() ;
        public TestReport() 
        {
            
        }
        public TestReport(TestStatus status, string testName, string testStepName,string message)
        {
            Status = status;
            TestName = testName;
            TestStepname = testStepName;
            Message = message;
        }
    }

}
