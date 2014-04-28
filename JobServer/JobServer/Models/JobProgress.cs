using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobServer.Executables;

namespace JobServer.Models
{
    /// <summary>
    /// Describes the progress of a job
    /// </summary>
    public class JobProgress
    {
        public int JobId { get; set; }
        public bool Started { get; set; }
        public bool Completed { get; set; }
        public bool Paused { get; set; }
        public float Progress { get; set; }

        /// <summary>
        /// Creates a Progress report object from a given StoredJob object
        /// </summary>
        /// <param name="job">Job to report on</param>
        /// <returns>New JobProgress object</returns>
        public static JobProgress CreateFromStored(StoredJob job)
        {
            JobProgress progress = new JobProgress();

            progress.JobId = job.JobId;

            progress.Completed = job.Completed;
            progress.Started = job.Started;
            progress.Paused = job.Paused;

            if (job.Completed) progress.Progress = 1;
            else
            {
                progress.Progress = (float)job.BatchIndex / (float)job.Images.Length;
            }

            return progress;
        }
    }
}