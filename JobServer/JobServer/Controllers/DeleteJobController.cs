using JobServer.Executables;
using Newtonsoft.Json;
using System.Web.Http;
using JobServer.Models;

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
        public GenericResponse Get(int id)
        {
            if (ProcessManager.RemoveJob(id))
            {
                return GenericResponse.Success(id);
            }
            else
            {
                return GenericResponse.Failure("Unable to delete job");
            }
        }
    }
}
