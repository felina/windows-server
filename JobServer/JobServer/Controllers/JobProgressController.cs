using JobServer.Executables;
using JobServer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JobServer.Controllers
{
    /// <summary>
    /// Provides an API endpoint for retrieving the progress on a job
    /// </summary>
    public class JobProgressController : ApiController
    {
        // GET api/jobprogress/id
        /// <summary>
        /// GET api/jobprogress/id. Returns OK with a JSON JobProgress object describing
        /// the progress of the given job, or NotFound if the job does not exist.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Get(int id)
        {
            if (ProcessManager.JobCached(id))
            {
                return Ok(JobProgress.CreateFromStored(ProcessManager.GetJob(id)));
            }
            else
            {
                return NotFound();
            }
        }
    }
}