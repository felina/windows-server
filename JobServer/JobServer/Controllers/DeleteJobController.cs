using Amazon.S3;
using Amazon.S3.Model;
using JobServer.Executables;
using JobServer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using JobServer.App_Code;
using System.Web;
using System.IO;

namespace JobServer.Controllers
{
    public class DeleteJobController : ApiController
    {
        // api/deletejob/id
        public IHttpActionResult Get(int id)
        {
            if (ProcessManager.RemoveJob(id))
            {
                return Ok();
            }
            else
            {
                return BadRequest("Job in progress or non-existent");
            }
        }
    }
}
