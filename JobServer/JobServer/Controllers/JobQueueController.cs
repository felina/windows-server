using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using JobServer.App_Code;
using Newtonsoft.Json;

namespace JobServer.Controllers
{
    public class JobQueueController : ApiController
    {
        // GET api/jobqueue
        public string Get()
        {
            int[] remaining = JobQueue.LeftOnQueue();
            return JsonConvert.SerializeObject(remaining);
        }
    }
}
