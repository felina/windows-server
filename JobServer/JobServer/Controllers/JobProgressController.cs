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
    public class JobProgressController : ApiController
    {
        // GET api/jobprogress/id
        public JobProgress Get(int id)
        {
            if (ProcessManager.Jobs.ContainsKey(id))
            {
                return JobProgress.CreateFromStored(ProcessManager.Jobs[id]);
            }
            else
            {
                return null;
            }
        }
    }
}