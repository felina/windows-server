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
    public class JobProgressController : ApiController
    {
        // GET api/jobprogress/id
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