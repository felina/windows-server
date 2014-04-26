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
    /// <summary>
    /// Provides an API endpoint for retrieving the results of a given job
    /// </summary>
    public class JobResultsController : ApiController
    {
        /// <summary>
        /// GET api/jobresults/id. Returns OK with a JSON JobResult object describing
        /// the job's results, or NotFound if the job does not exist.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetResult(int id)
        {
            if (ProcessManager.JobCached(id))
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
