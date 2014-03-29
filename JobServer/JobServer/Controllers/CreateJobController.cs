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
    public class CreateJobController : ApiController
    {
        // POST api/createjob
        public IHttpActionResult Post([FromBody]Job value)
        {
            // Check our input
            if (value == null)
            {
                Debug.WriteLine("CreateJob POST: Nothing recieved");
                return BadRequest("Invalid or missing job definition");
            }

            // Check for existing job
            if (!ProcessManager.JobExists(value.JobId))
            {
                Debug.WriteLine("Storing new Job");
                ProcessManager.AddJob(new StoredJob(value));
                return Ok("New job " + value.JobId + " stored");
            }
            else return BadRequest("Job already exists");
        }
    }
}
