using System;
using System.Collections.Generic;
using System.IO;

namespace WSyncPro.Test
{
    public sealed class WReporter
    {
        private static readonly Lazy<WReporter> instance = new Lazy<WReporter>(() => new WReporter());
        public static WReporter Instance => instance.Value;

        private List<string> _reportLines;

        private WReporter()
        {
            _reportLines = new List<string>();
        }

        public void AddReportLine(string line)
        {
            _reportLines.Add(line);
        }

        public void AddReportLines(List<string> lines)
        {
            _reportLines.AddRange(lines);
        }

        public void SaveReport(string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, append: true))
                {
                    foreach (var line in _reportLines)
                    {
                        writer.WriteLine(line);
                    }
                }

                // Clear the report lines after saving
                DumpReport();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving report: {ex.Message}");
            }
        }

        public void DumpReport()
        {
            _reportLines.Clear();
        }
    }
}
