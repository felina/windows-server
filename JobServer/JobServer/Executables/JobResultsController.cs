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
    public class JobResultsController : ApiController
    {
        /*public IEnumerable<JobResult> GetAllResults()
        {
            JobResult[] results = new JobResult[ProcessManager.Jobs.Count];
            // TODO... ?
        }*/

        public string GetResult(int id)
        {
            if (ProcessManager.JobCached(id))
            {
                // Make sure job is actually completed before sending results
                if (!ProcessManager.GetJob(id).Completed)
                {
                    return "Job not completed";
                }
                var result = JobResult.CreateFromStored(ProcessManager.GetJob(id));
                //return Ok(result);
                return "OK";
            }
            else
            {
                return "Not Found";
            }
        }
    }
}
