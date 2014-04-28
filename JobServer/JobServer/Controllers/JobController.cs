using JobServer.Executables;
using JobServer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JobServer.Controllers
{
    public class JobController : ApiController
    {
        // POST api/job
        //public IHttpActionResult Post(String data)
        //{
        //}
        
        // GET api/job/5
        //public string Get(int id)
        //{
        //}

        // PUT api/job/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/job/5
        public void Delete(int id)
        {
        }

        // TODO: Job control - pause, halt etc

        // Pauses the specified job
        public string Pause(int id)
        {
            StoredJob job = ProcessManager.GetJob(id);
            if (job == null)
            {
                return "Job not stored";
            }
            if (job.Completed)
            {
                return "Job already completed";
            }
            else if (!job.Started)
            {
                return "Job not started";
            }
            else
            {
                Action c = delegate { }; //Pause the job here, if already paused do nothing
                c();
                return "Job Paused";
            }
        }

        // Stops the specified job
        public string Stop(int id)
        {
            StoredJob job = ProcessManager.GetJob(id);
            if (job == null)
            {
                return "Job not stored";
            }
            if (job.Started) {
                Action d = delegate {}; //Stop the job here
                d();
                return "Job Stopped";
            }
            else
            {
                return "Job Stopped":
            }
        }



    }
}
