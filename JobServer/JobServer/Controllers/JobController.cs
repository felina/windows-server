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
        public void Post(String data)
        {
            Job inputJob = JsonConvert.DeserializeObject<Job>(data);
            String ExecutablePath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/Jobs/" + inputJob.jobId + "/Extracted/" + inputJob.ZipId);
            //Debug.WriteLine(inputJob.JobId);
        }
        
        // GET api/job/5
        //public string Get(int id)
        //{
        //    Debug.WriteLine("Job " + id + " requested");
        //    return "value";
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
    }
}
