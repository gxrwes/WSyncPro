using System;
using System.Collections.Generic;
using WSyncPro.Models.State;

namespace WSyncPro.Models
{
    /// <summary>
    /// Represents a job within the application.
    /// </summary>
    public class Job
    {
        public DateTime Created { get; set; }
        public DateTime LastRun { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier of the job.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the job.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the job.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the job is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the source directory for the job.
        /// </summary>
        public string SrcDirectory { get; set; }

        /// <summary>
        /// Gets or sets the destination directory for the job.
        /// </summary>
        public string DstDirectory { get; set; }

        /// <summary>
        /// Gets or sets the total number of files to sync.
        /// </summary>
        public int FilesToSync { get; set; }

        /// <summary>
        /// Gets or sets the total number of files synced successfully.
        /// </summary>
        public int TotalFilesSynced { get; set; }

        /// <summary>
        /// Gets or sets the total number of files that failed to sync.
        /// </summary>
        public int FailedFiles { get; set; }

        /// <summary>
        /// Gets or sets the included wildcard patterns.
        /// </summary>
        public List<string> InclWilcardString { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the excluded wildcard patterns.
        /// </summary>
        public List<string> ExclWildcardString { get; set; } = new List<string>();

        /// <summary>
        /// Gets the history of job progress.
        /// </summary>
        public List<JobProgress> ProgressHistory { get; private set; } = new List<JobProgress>();

        /// <summary>
        /// Updates the job progress by adding a new <see cref="JobProgress"/> instance.
        /// </summary>
        /// <param name="progress">The <see cref="JobProgress"/> instance to add.</param>
        public void AddProgress(JobProgress progress)
        {
            if (progress != null)
            {
                ProgressHistory.Add(progress);
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current job.
        /// </summary>
        /// <param name="obj">The object to compare with the current job.</param>
        /// <returns><c>true</c> if the specified object is equal to the current job; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Job other)
            {
                return Id == other.Id &&
                       Name == other.Name &&
                       Description == other.Description &&
                       IsEnabled == other.IsEnabled &&
                       SrcDirectory == other.SrcDirectory &&
                       DstDirectory == other.DstDirectory &&
                       FilesToSync == other.FilesToSync &&
                       TotalFilesSynced == other.TotalFilesSynced &&
                       FailedFiles == other.FailedFiles &&
                       InclWilcardString.SequenceEqual(other.InclWilcardString) &&
                       ExclWildcardString.SequenceEqual(other.ExclWildcardString);
            }
            return false;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current job.</returns>
        public override int GetHashCode()
        {
            // Combine hash codes of relevant properties
            int hash = 17;
            hash = hash * 23 + (Id?.GetHashCode() ?? 0);
            hash = hash * 23 + (Name?.GetHashCode() ?? 0);
            hash = hash * 23 + (Description?.GetHashCode() ?? 0);
            hash = hash * 23 + IsEnabled.GetHashCode();
            hash = hash * 23 + (SrcDirectory?.GetHashCode() ?? 0);
            hash = hash * 23 + (DstDirectory?.GetHashCode() ?? 0);
            hash = hash * 23 + FilesToSync.GetHashCode();
            hash = hash * 23 + TotalFilesSynced.GetHashCode();
            hash = hash * 23 + FailedFiles.GetHashCode();
            foreach (var pattern in InclWilcardString)
            {
                hash = hash * 23 + (pattern?.GetHashCode() ?? 0);
            }
            foreach (var pattern in ExclWildcardString)
            {
                hash = hash * 23 + (pattern?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
}
