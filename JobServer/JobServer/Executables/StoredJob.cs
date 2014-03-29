using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobServer.Models;

namespace JobServer.Executables
{
    public class StoredJob
    {
        public int JobId;
        public int ZipId;
        public string[] Images;
        public bool Started = false;
        public bool Completed = false;
        public int BatchIndex = 0;
        public int? ExitCode = null;
        public string Result = "";
        public string Errors = "";

        // Copies an input job's parameters
        public StoredJob(Job job)
        {
            JobId = job.JobId;
            ZipId = job.ZipId;
            Images = new string[job.Images.Length];
            job.Images.CopyTo(Images, 0);
        }
    }
}