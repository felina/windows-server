using JobServer.Executables;
using JobServer.Models;
using Newtonsoft.Json;
using System.Web.Http;
using System.Web;

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
        public JobProgress Get(int id)
        {
            if (ProcessManager.JobCached(id))
            {
                StoredJob job = ProcessManager.GetJob(id);
                return JobProgress.CreateFromStored(job);
            }
            else
            {
                return JobProgress.CreateFailResponse("Job not found");
            }
        }
    }
}