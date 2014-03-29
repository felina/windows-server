using JobServer.Models;
using JobServer.Executables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JobServer.Controllers
{
    public class JobResultsController : ApiController
    {
        /*public IEnumerable<JobResult> GetAllResults()
        {
            JobResult[] results = new JobResult[ProcessManager.Jobs.Count];
            // TODO... ?
        }*/

        public IHttpActionResult GetResult(int id)
        {
            if (ProcessManager.JobExists(id))
            {
                var result = JobResult.CreateFromStored(ProcessManager.GetJob(id));
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
