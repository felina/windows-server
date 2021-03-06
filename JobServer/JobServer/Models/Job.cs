﻿namespace JobServer.Models
{
    /// <summary>
    /// Describes a job to be created (input to createjob)
    /// </summary>
    public class Job
    {
        public int JobId { get; set; }
        public int ZipId { get; set; }
        public string Command { get; set; }
        public WorkArray[] Work { get; set; }
    }
}