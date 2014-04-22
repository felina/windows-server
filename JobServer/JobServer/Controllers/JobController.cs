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
    }
}
