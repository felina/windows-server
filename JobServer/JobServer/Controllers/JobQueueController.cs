using JobServer.App_Code;
using Newtonsoft.Json;
using System.Web.Http;

namespace JobServer.Controllers
{
    /// <summary>
    /// Provides an API endpoint for retrieving the jobs left on the execution Queue.
    /// </summary>
    public class JobQueueController : ApiController
    {
        /// <summary>
        /// GET api/jobqueue. Returns a JSON response containing an array of JobIDs left on the execution Queue.
        /// </summary>
        /// <returns>Array of JobIDs on the execution Queue.</returns>
        public int[] Get()
        {
            int[] remaining = JobQueue.LeftOnQueue();
            return remaining;
        }
    }
}
