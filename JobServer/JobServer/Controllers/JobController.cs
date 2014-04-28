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
        public string Post([FromBody] JobControl value)
        {
            if (value == null)
            {
                return "Invalid request";
            }
            int id = value.JobId;
            string option = value.Option;
            StoredJob job = ProcessManager.GetJob(id);
            if (job == null)
            {
                return "Job not stored";
            }
            if (option == "PAUSE")
            {
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
                    Action c = delegate { job.Paused = true; }; //Pause the job here, if already paused do nothing
                    c();
                    return "Job Paused";
                }
            }
            else if (option == "RESUME")
            {
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
                    Action d = delegate { job.Paused = false; };
                    d();
                    return "Job Resumed";
                }
            }
            else if (option == "STOP") 
            {
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
                    Action e = delegate { job.Stopped = true; };
                    e();
                    return "Job Stopped";
                }
            }
            else if (option == "RESTART")
            {
                if (job.Completed)
                {
                    return "Job already completed";
                }
                else
                {
                    Action f = delegate { job.Restart = true; job.Paused = false; };
                    f();
                    return "Job restarted";
                }
            }
            return "Option not found";
        }

        // TODO: Job control - pause, halt etc

        // Pauses the specified job
        

        // Stops the specified job
        //public string Stop(int id)
        //{
        //    StoredJob job = ProcessManager.GetJob(id);
        //    if (job == null)
        //    {
        //        return "Job not stored";
        //    }
        //    if (job.Started) {
        //        Action d = delegate {}; //Stop the job here
        //        d();
        //        return "Job Stopped";
        //    }
        //    else
        //    {
        //        return "Job Stopped":
        //    }
        //}



    }
}
