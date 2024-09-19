using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WSyncPro.Models.Enums;

namespace WSyncPro.Models.State
{
    /// <summary>
    /// Represents the progress state of a job.
    /// </summary>
    public class JobProgress
    {
        /// <summary>
        /// Gets or sets the unique identifier of the job.
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// Gets or sets the name of the job.
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Gets or sets the total number of files to process.
        /// </summary>
        public int TotalFilesToProcess { get; set; }

        /// <summary>
        /// Gets or sets the number of files that have been successfully processed.
        /// </summary>
        public int FilesProcessedSuccessfully { get; set; }

        /// <summary>
        /// Gets or sets the number of files that are currently being processed.
        /// </summary>
        public int FilesCurrentlyProcessed { get; set; }

        /// <summary>
        /// Gets the number of files that have failed to process.
        /// This value is automatically updated based on the FailedFiles collection.
        /// </summary>
        public int FilesFailed { get; private set; }

        /// <summary>
        /// Gets the list of file paths that failed to process.
        /// </summary>
        public ObservableCollection<string> FailedFiles { get; private set; } = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the start time of the job.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the last updated time of the progress.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the current status of the job.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public JobStatus Status { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobProgress"/> class.
        /// Subscribes to the FailedFiles collection changes to update FilesFailed automatically.
        /// </summary>
        public JobProgress()
        {
            FailedFiles.CollectionChanged += FailedFiles_CollectionChanged;
        }

        /// <summary>
        /// Handles changes in the FailedFiles collection to update FilesFailed accordingly.
        /// </summary>
        private void FailedFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FilesFailed = FailedFiles.Count;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Calculates and returns the completion percentage of the job.
        /// </summary>
        /// <returns>A float representing the completion percentage (0 to 100).</returns>
        public float GetCompletionPercentage()
        {
            if (TotalFilesToProcess <= 0)
                return 0f;

            // Calculate the number of files processed (successful + failed)
            int totalProcessed = FilesProcessedSuccessfully + FilesFailed;

            // Calculate the percentage
            float percentage = ((float)totalProcessed / TotalFilesToProcess) * 100f;

            // Ensure the percentage does not exceed 100%
            return Math.Min(percentage, 100f);
        }

        /// <summary>
        /// Determines whether the specified <see cref="JobProgress"/> is equal to the current <see cref="JobProgress"/>.
        /// </summary>
        /// <param name="other">The <see cref="JobProgress"/> to compare with the current <see cref="JobProgress"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="JobProgress"/> is equal to the current <see cref="JobProgress"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is JobProgress other)
            {
                return JobId == other.JobId &&
                       JobName == other.JobName &&
                       TotalFilesToProcess == other.TotalFilesToProcess &&
                       FilesProcessedSuccessfully == other.FilesProcessedSuccessfully &&
                       FilesCurrentlyProcessed == other.FilesCurrentlyProcessed &&
                       FilesFailed == other.FilesFailed &&
                       StartTime == other.StartTime &&
                       LastUpdated == other.LastUpdated &&
                       Status == other.Status &&
                       FailedFiles.SequenceEqual(other.FailedFiles);
            }
            return false;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current <see cref="JobProgress"/>.</returns>
        public override int GetHashCode()
        {
            // Combine hash codes of relevant properties
            int hash = 17;
            hash = hash * 23 + (JobId?.GetHashCode() ?? 0);
            hash = hash * 23 + (JobName?.GetHashCode() ?? 0);
            hash = hash * 23 + TotalFilesToProcess.GetHashCode();
            hash = hash * 23 + FilesProcessedSuccessfully.GetHashCode();
            hash = hash * 23 + FilesCurrentlyProcessed.GetHashCode();
            hash = hash * 23 + FilesFailed.GetHashCode();
            hash = hash * 23 + StartTime.GetHashCode();
            hash = hash * 23 + LastUpdated.GetHashCode();
            hash = hash * 23 + Status.GetHashCode();
            foreach (var file in FailedFiles)
            {
                hash = hash * 23 + (file?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
}
