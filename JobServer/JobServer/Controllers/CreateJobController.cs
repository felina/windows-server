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
        public string Post([FromBody]Job value)
        {
            // Check our input
            if (value == null)
            {
                Debug.WriteLine("CreateJob POST: Nothing recieved");
                return "Invalid or missing input";
            }

            // Check for existing job
            if (!ProcessManager.Jobs.ContainsKey(value.JobId))
            {
                Debug.WriteLine("Storing new Job");
                ProcessManager.Jobs[value.JobId] = new StoredJob(value);
                return "New job " + value.JobId + " stored";
            }
            else return "Job already exists";
        }
    }
}
