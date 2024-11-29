using System;
using System.Collections.Generic;
using System.Linq;

namespace WSyncPro.Core.Models
{
    public class DbFile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public string FilePath { get; set; }
        public string Filetype { get; set; }
        public string Autor { get; set; }
        public string PreviousFileLocation { get; set; }

        // History stored as a single string for database compatibility
        public string History { get; set; } = string.Empty;

        /// <summary>
        /// Parses the History field into a list of strings.
        /// </summary>
        /// <returns>A List of history entries.</returns>
        public List<string> GetHistory()
        {
            if (string.IsNullOrEmpty(History))
            {
                return new List<string>();
            }
            return History.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// Replaces the current history with a new list of history entries.
        /// </summary>
        /// <param name="newHistory">A List of history entries.</param>
        public void SetHistory(List<string> newHistory)
        {
            if (newHistory == null || !newHistory.Any())
            {
                History = string.Empty;
            }
            else
            {
                History = string.Join("||", newHistory);
            }
        }

        /// <summary>
        /// Adds a new entry to the history.
        /// </summary>
        /// <param name="history">The history entry to add.</param>
        public void AddToHistory(string history)
        {
            if (string.IsNullOrWhiteSpace(history))
            {
                return;
            }

            var currentHistory = GetHistory();
            currentHistory.Add(history);
            SetHistory(currentHistory);
        }
    }
}
