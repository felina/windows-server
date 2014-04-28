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
using JobServer.App_Code;

namespace JobServer.Controllers
{
    public class JobController : ApiController
    {
        // POST api/job
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
                //POSSIBLY CREATE JOB??
                return "Job not stored";
            }
            if (job.Completed)
            {
                return "Job already completed";
            }
            if (!job.Started)
            {
                return "Job not started";
            }
            if (option == "PAUSE")
            {
                Action c = delegate { job.Paused = true; }; //Pause the job here, if already paused do nothing
                c();
                return "Job Paused";
            }
            else if (option == "RESUME")
            {
                Action d = delegate { job.Paused = false; };
                d();
                return "Job Resumed";
            }
            else if (option == "STOP")
            {
                //NEED TO REMOVE FROM QUEUE
                Action e = delegate { job.Stopped = true; };
                e();
                return "Job Stopped";
            }
            else if (option == "RESTART")
            {
                if (job.Completed)
                {
                    return "Job already completed";
                }
                else
                {
                    Debug.WriteLine(job.Command);
                    Action f = delegate { job.Restart = true; job.Stopped = false; job.Paused = false; ProcessManager.RunJob(job.Command, job.JobId); };
                    f();
                    return "Job restarted";
                }
            }
            return "Option not found";
        }
    }
}
