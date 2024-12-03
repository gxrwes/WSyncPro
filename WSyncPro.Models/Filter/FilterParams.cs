using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Models.Filter
{
    public class FilterParams
    {
        public List<string> Include = new List<string>();
        public List<string> Exclude = new List<string>();
        public int MaxFileSize = -1;
        public int MinFileSize = -1;
        public List<string> FileTypes = new List<string>();
        public string ToString()
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
            StringBuilder sb = new StringBuilder();
            foreach (string s in Include)
            {
                sb.Append(s);
                sb.Append(", ");
            }
            return sb.ToString();

        }
        public string ExcludeToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in Exclude)
            {
                sb.Append(s);
                sb.Append(", ");
            }
            return sb.ToString();

        }

    }
}
