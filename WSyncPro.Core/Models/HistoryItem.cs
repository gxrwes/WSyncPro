using System;
using System.Collections.Generic;
using System.Linq;

namespace WSyncPro.Core.Models
{
    public class HistoryItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
        public string OriginPath { get; set; }
        public string NerPath { get; set; }
        public OperationType OperationType { get; set; }
        public string LinkedFileId { get; set; }
    }
}
