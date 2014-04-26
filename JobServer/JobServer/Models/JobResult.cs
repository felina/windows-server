using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobServer.Executables;

namespace JobServer.Models
{
    /// <summary>
    /// Describes the results of a job
    /// </summary>
    public class JobResult
    {
        public int JobId { get; set; }
        public WorkArray[] Images { get; set; }
        public int? ExitCode { get; set; }
        public string Result { get; set; }
        public string Errors { get; set; }

        /// <summary>
        /// Creates a new Job Result object reporting the results of a given job
        /// </summary>
        /// <param name="job">StoredJob representing the job to report on</param>
        /// <returns>New JobResult object</returns>
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