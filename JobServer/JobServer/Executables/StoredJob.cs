﻿using JobServer.Models;
using System.Diagnostics;

namespace JobServer.Executables
{
    /// <summary>
    /// Information about a job as stored in memory
    /// </summary>
    public class StoredJob
    {
        public int JobId;
        public int ZipId;
        public string Bucket;
        public string Command;
        public WorkArray[] Images;
        public bool Started = false;
        public bool Completed = false;
        public bool Paused = false;
        public bool Stopped = false;
        public int BatchIndex = 0;
        public int? ExitCode = null;
        public Process exeProcess = null;

        /// <summary>
        /// Copies a Job Model object's parameters to a new StoredJob object
        /// </summary>
        /// <param name="job">Job to copy</param>
        public StoredJob(Job job)
        {
            JobId = job.JobId;
            ZipId = job.ZipId;
            Command = job.Command;
            Images = new WorkArray[job.Work.Length];
            job.Work.CopyTo(Images, 0);
        }
    }
}