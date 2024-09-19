// BaseTest.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WSyncPro.Tests.Core.Services
{
    public abstract class BaseTest : IDisposable
    {
        protected readonly string ConfirmedPatternsFilePath;

        protected BaseTest()
        {
            // Define the path for ConfirmedWildcardPatterns.md
            ConfirmedPatternsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfirmedWildcardPatterns.md");

            // Initialize the ConfirmedWildcardPatterns.md file if it doesn't exist
            if (!File.Exists(ConfirmedPatternsFilePath))
            {
                File.WriteAllText(ConfirmedPatternsFilePath, "# Confirmed Wildcard Patterns\n\n## Include Patterns\n\n## Exclude Patterns\n\n");
            }
        }

        public void Dispose()
        {
            // Optionally, clean up resources or reset states if necessary
            // For example, delete the ConfirmedWildcardPatterns.md file after tests
            // File.Delete(ConfirmedPatternsFilePath);
        }

        /// <summary>
        /// Saves a confirmed wildcard pattern to the ConfirmedWildcardPatterns.md file.
        /// Ensures that each pattern is logged only once.
        /// </summary>
        /// <param name="pattern">The wildcard pattern.</param>
        /// <param name="isInclude">True if the pattern is an include pattern; otherwise, false.</param>
        protected void SaveConfirmedPattern(string pattern, bool isInclude)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return;

            try
            {
                var lines = File.ReadAllLines(ConfirmedPatternsFilePath).ToList();
                string sectionHeader = isInclude ? "## Include Patterns" : "## Exclude Patterns";
                int sectionIndex = lines.FindIndex(line => line.Trim().Equals(sectionHeader, StringComparison.OrdinalIgnoreCase));

                if (sectionIndex == -1)
                {
                    // Section not found, append it
                    lines.Add("");
                    lines.Add(sectionHeader);
                    lines.Add("");
                    sectionIndex = lines.Count - 1;
                }

                // Find the next section or end of file
                int insertIndex = lines.FindIndex(sectionIndex + 1, line => line.StartsWith("## ", StringComparison.OrdinalIgnoreCase));
                if (insertIndex == -1)
                    insertIndex = lines.Count;

                // Check if the pattern is already listed
                bool alreadyExists = lines.Skip(sectionIndex).Take(insertIndex - sectionIndex)
                    .Any(line => line.Trim().Equals($"- `{pattern}`", StringComparison.OrdinalIgnoreCase));

                if (!alreadyExists)
                {
                    lines.Insert(insertIndex, $"- `{pattern}`");
                    File.WriteAllLines(ConfirmedPatternsFilePath, lines);
                }
            }
            catch (Exception ex)
            {
                // Optionally, handle exceptions (e.g., log them)
                Console.WriteLine($"Error writing to ConfirmedWildcardPatterns.md: {ex.Message}");
            }
        }
    }
}
