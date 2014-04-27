using JobServer.Executables;
using System.Web.Http;
using Newtonsoft.Json;

namespace JobServer.Controllers
{
    /// <summary>
    /// Provides a job deletion API endpoint
    /// </summary>
    public class DeleteJobController : ApiController
    {
        /// <summary>
        /// Called with api/deletejob/id. Deletes the given job if it exists and is not in progress.
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <returns>JSON Response ('res' indicates success or failure)</returns>
        public string Get(int id)
        {
            if (ProcessManager.RemoveJob(id))
            {
                return JsonConvert.SerializeObject(new
                    {
                        res = true,
                        jobId = id
                    });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                    {
                        res = false,
                        message = "Unable to delete job"
                    });
            }
        }
    }
}
