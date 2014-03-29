using JobServer.App_Start;
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
    public class JobController : ApiController
    {
        // GET api/job
        /*public JobResult Get()
        {
            return ProcessManager.RunJob("C:\\Users\\narayn\\Documents\\GitHub\\windows-server\\JobServer\\JobServer\\bin\\TestExecutable.exe", "Hiaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "Hiaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            //return new string[] { "value1", "value2" };
        }

        // GET api/job/5
        public string Get(int id)
        {
            Debug.WriteLine("Job " + id + " requested");
            return "value";
        }

        // PUT api/job/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/job/5
        public void Delete(int id)
        {
        }*/

        // TODO: Job control - pause, halt etc
    }
}
