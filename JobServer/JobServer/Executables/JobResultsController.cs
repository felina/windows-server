using JobServer.Models;
using JobServer.Executables;
using System.Web.Http;

namespace JobServer.Controllers
{
    /// <summary>
    /// Provides an API endpoint for retrieving the results of a given job
    /// </summary>
    public class JobResultsController : ApiController
    {

        /// <summary>
        /// GET api/jobresults/id. Returns a JSON JobResult object describing
        /// the job's results, or an error message.
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <returns>JSON Response</returns>
        public string GetResult(int id)
        {
            if (ProcessManager.JobCached(id))
            {
                // Make sure job is actually completed before sending results
                if (!ProcessManager.GetJob(id).Completed)
                {
                    return "Job not completed";
                }
                var result = JobResult.CreateFromStored(ProcessManager.GetJob(id));
                return "OK";
            }
            else
            {
                return "Not Found";
            }
        }
    }
}
