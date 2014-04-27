    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobServer.Executables;

namespace JobServer.Models
{
    public class JobResult
    {
        public int JobId { get; set; }
        public WorkArray[] Images { get; set; }
        public int? ExitCode { get; set; }
        public string Result { get; set; }
        public string Errors { get; set; }

        public static JobResult CreateFromStored(StoredJob job)
        {
            JobResult result = new JobResult();

            result.Images = new WorkArray[job.Images.Length];
            job.Images.CopyTo(result.Images, 0);

            result.JobId = job.JobId;
            result.ExitCode = job.ExitCode;
            result.Result = job.Result;
            result.Errors = job.Errors;

            return result;
        }
    }
}