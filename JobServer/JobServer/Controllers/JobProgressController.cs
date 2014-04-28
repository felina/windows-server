using JobServer.Executables;
using JobServer.Models;
using Newtonsoft.Json;
using System;
using System.Web.Http;

namespace JobServer.Controllers
{
    /// <summary>
    /// Provides an API endpoint for retrieving the progress on a job
    /// </summary>
    public class JobProgressController : ApiController
    {
        /// <summary>
        /// GET api/jobprogress/id. Returns a JSON response indicating the job's progress.
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <returns>JSON Response</returns>
        public string Get(int id)
        {
            string result;
            if (ProcessManager.JobCached(id))
            {
                StoredJob job = ProcessManager.GetJob(id);
                JobProgress jobProgress = JobProgress.CreateFromStored(job);
                result = JsonConvert.SerializeObject(new
                {
                    res = true,
                    jobId = id,
                    Started = jobProgress.Started,
                    Completed = jobProgress.Completed,
                    Progress = jobProgress.Progress
                });
            }
            else
            {
                result = JsonConvert.SerializeObject(new
                {
                    res = false,
                    message = "Job doesn't exist on server"
                });
            }
            return result;
        }
    }
}