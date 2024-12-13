using System;
using System.Collections.Generic;
using System.Text;

namespace WSyncPro.Models.Filter
{
    public class FilterParams
    {
        public List<string> Include { get; set; } = new List<string>();
        public List<string> Exclude { get; set; } = new List<string>();
        public int MaxFileSize { get; set; } = -1;
        public int MinFileSize { get; set; } = -1;
        public List<string> FileTypes { get; set; } = new List<string>();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Include: ");
            sb.Append(this.IncludeToString());
            sb.Append("\n");
            sb.Append("Exclude: ");
            sb.Append(this.ExcludeToString());
            return sb.ToString();
        }

        public string IncludeToString()
        {
            return string.Join(", ", Include);
        }

        public string ExcludeToString()
        {
            return string.Join(", ", Exclude);
        }
    }
}
