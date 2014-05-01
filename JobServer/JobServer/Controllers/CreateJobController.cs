using Amazon.S3;
using Amazon.S3.Model;
using JobServer.App_Code;
using JobServer.Executables;
using JobServer.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Http;

namespace JobServer.Controllers
{
    /// <summary>
    /// Provides a job creation API endpoint
    /// </summary>
    public class CreateJobController : ApiController
    {
        /// <summary>
        /// <para>
        /// POST api/createjob. Accepts a JSON Job Model object, which, if valid, is stored.
        /// The job is stored in memory in the ProcessManager, and cached to the hard drive.
        /// </para>
        /// <para>
        /// Jobs created this way are queued for execution - their progress and results can
        /// be checked using the otehr api methods.
        /// </para>
        /// <para>
        /// The Job's ID must not already be in use on the server.
        /// </para>
        /// </summary>
        /// <param name="value">JSON representing job</param>
        /// <returns>JSON Response indicating success</returns>
        public GenericResponse Post([FromBody]Job value)
        {
            // Check our input
            if (value == null)
            {
                return GenericResponse.Failure("Invalid or missing job definition");
            }

            // Check for existing job
            if (!ProcessManager.JobCached(value.JobId))
            {
                Debug.WriteLine("Caching new Job");
                ProcessManager.AddJob(value);
                Debug.WriteLine("Job stored");

                // Queue the job
                Action a = delegate { JobQueue.AddToQueue(value.Command, value.JobId); }; //Change to actual name 
                a();
                return GenericResponse.Success(value.JobId);
            }
            else
            {
                return GenericResponse.Failure("Job already cached");
            }
        }
    }
}
