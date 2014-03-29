using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobServer.Executables;

namespace JobServer.Models
{
    public class JobProgress
    {
        public int JobId { get; set; }
        public bool Started { get; set; }
        public bool Completed { get; set; }
        public float Progress { get; set; }

        public static JobProgress CreateFromStored(StoredJob job)
        {
            JobProgress progress = new JobProgress();
            progress.Completed = job.Completed;
            progress.Started = job.Started;
            progress.Progress = (float)job.Images.Length / (float)job.BatchIndex;

            return progress;
        }
    }
}