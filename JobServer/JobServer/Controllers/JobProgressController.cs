using JobServer.Executables;
using JobServer.Models;
using Newtonsoft.Json;
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
        public string Get(int id)
        {
            if (ProcessManager.JobCached(id))
            {
                StoredJob job = ProcessManager.GetJob(id);
                JobProgress jobProgress = JobProgress.CreateFromStored(job);
                //return Ok(JobProgress.CreateFromStored(ProcessManager.GetJob(id)));
                String result = JsonConvert.SerializeObject(new
                {
                    jobId = id,
                    Started = jobProgress.Started,
                    Completed = jobProgress.Completed,
                    Progress = jobProgress.Progress
                });
                //return Ok("New job " + value.JobId + " stored");
                return result;
            }
            else
            {
                return "Job doesn't exist on server"; //res false?
            }
        }
    }
}