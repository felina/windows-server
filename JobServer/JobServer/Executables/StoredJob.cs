using JobServer.Models;

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
        public WorkArray[] Images;
        public bool Started = false;
        public bool Completed = false;
        public int BatchIndex = 0;
        public int? ExitCode = null;
        public string Result = "";
        public string Errors = "";

        /// <summary>
        /// Copies a Job Model object's parameters to a new StoredJob object
        /// </summary>
        /// <param name="job">Job to copy</param>
        public StoredJob(Job job)
        {
            JobId = job.JobId;
            ZipId = job.ZipId;
            Images = new WorkArray[job.Work.Length];
            job.Work.CopyTo(Images, 0);
        }
    }
}