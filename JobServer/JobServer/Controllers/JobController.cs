﻿using JobServer.Executables;
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
            String result;
            if (value == null)
            {
                result = JsonConvert.SerializeObject(new
                {
                    res = true,
                    message = "Invalid request"
                });
                return result;
            }
            int id = value.JobId;
            string option = value.Option;
            StoredJob job = ProcessManager.GetJob(id);
            if (job == null)
            {
                result = JsonConvert.SerializeObject(new
                {
                    res = false,
                    message = "Job not stored"
                });
            }
            if (job.Completed)
            {
                result = JsonConvert.SerializeObject(new
                {
                    res = false,
                    message = "Job already completed"
                });
            }
            if (!job.Started)
            {
                result = JsonConvert.SerializeObject(new
                {
                    res = false,
                    message = "Job not started"
                });
            }
            if (option == "PAUSE")
            {
                Action c = delegate { job.Paused = true; }; //Pause the job here, if already paused do nothing
                c();
                result = JsonConvert.SerializeObject(new
                {
                    res = true,
                    message = "Job Paused"
                });
            }
            else if (option == "RESUME")
            {
                Action d = delegate { job.Paused = false; };
                d();
                result = JsonConvert.SerializeObject(new
                {
                    res = true,
                    message = "Job Resumed"
                });
            }
            else if (option == "STOP")
            {
                Action e = delegate { job.Stopped = true; };
                e();
                result = JsonConvert.SerializeObject(new
                {
                    res = true,
                    message = "Job Stopped"
                });
            }
            else if (option == "RESTART")
            {
                Action f = delegate { job.Stopped = false; job.Paused = false; ProcessManager.RunJob(job.Command, job.JobId); };
                f();
                result = JsonConvert.SerializeObject(new
                {
                    res = true,
                    message = "Job restarted"
                });
            }
            else
            {
                result = JsonConvert.SerializeObject(new
                {
                    res = false,
                    message = "Option not found"
                });
            }
            return result;
        }
    }
}
