using JobServer.Models;
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
        private JobResult[] placeholderResults = new JobResult[]
        {
            new JobResult { JobId = 1, ImageA = "0deae28a26727ebe30ecf2896e5862f1", ImageB = "2d1dac96639c5e6f6246f9315625ccbc", ExitCode = 0, Result = "0.76" },
            new JobResult { JobId = 1, ImageA = "0deae28a26727ebe30ecf2896e5862f1", ImageB = "30f811776cb26a19a693b6dcaa165a30", ExitCode = 0, Result = "0.35" }
        };

        public IEnumerable<JobResult> GetAllProducts()
        {
            return placeholderResults;
        }

        public IHttpActionResult GetProduct(int id)
        {
            var result = placeholderResults.FirstOrDefault((p) => p.JobId == id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
