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
    /// <summary>
    /// Provides a job deletion API endpoint
    /// </summary>
    public class DeleteJobController : ApiController
    {
        /// <summary>
        /// Called with api/deletejob/id. Deletes the given job if it exists and is not in progress.
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <returns>OK if the job was deleted, BadRequest otherwise</returns>
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
