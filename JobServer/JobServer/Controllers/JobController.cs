using JobServer.App_Code;
using JobServer.Executables;
using JobServer.Models;
using Newtonsoft.Json;
using System;
using System.Web.Http;

namespace JobServer.Controllers
{
    /// <summary>
    /// Provides api endpoints for stopping, pausing, resuming and restarting jobs.
    /// </summary>
    public class JobController : ApiController
    {
        /// <summary>
        /// <para>POST api/job</para>
        /// <para></para>
        /// </summary>
        /// <param name="value">JobControl model object input</param>
        /// <returns>JSON GenericResponse indicating success or failure</returns>
        public GenericResponse Post([FromBody] JobControl value)
        {
            if (value == null)
            {
                return GenericResponse.Failure("Invalid request");
            }

            int id = value.JobId;
            string option = value.Option;
            StoredJob job = ProcessManager.GetJob(id);

            if (job == null)
            {
                return GenericResponse.Failure("Job not stored");
            }

            if (job.Completed)
            {
                return GenericResponse.Failure("Job already completed");
            }

            if (!job.Started)
            {
                return GenericResponse.Failure("Job not started");
            }

            if (option == "PAUSE")
            {
                Action c = delegate { job.Paused = true; }; //Pause the job here, if already paused do nothing
                c();
                return GenericResponse.Success(value.JobId);
            }
            else if (option == "RESUME")
            {
                Action d = delegate { job.Paused = false; };
                d();
                return GenericResponse.Success();
            }
            else if (option == "STOP")
            {
                Action e = delegate {
                    job.Stopped = true;
                    if (job.exeProcess != null && !job.exeProcess.HasExited)
                    {
                        job.exeProcess.Kill();
                    }
                };
                e();

                return GenericResponse.Success();
            }
            else if (option == "RESTART")
            {
                Action f = delegate { job.Stopped = false; job.Paused = false; JobQueue.AddToQueue(job.Command, job.JobId); };
                f();

                return GenericResponse.Success();
            }
            else
            {
                return GenericResponse.Failure("Option does not exist");
            }
        }
    }
}
