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
        static IAmazonS3 client;

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

        /// <summary>
        /// <para>
        /// Retrieves the Job's .zip from AWS and extracts it to the App_Data/Jobs directory under the jobId.
        /// The job's Work allocation is also stored here as a text file.
        /// </para>
        /// <para>
        /// Jobs cached in this way can later be retrieved - the ProcessManager does this automatically
        /// when a job cannot be found in memory.
        /// </para>
        /// </summary>
        /// <param name="job">Job Model object to cache</param>
        public static void CacheJob(Job job)
        {
            if (!AWS.checkRequiredFields()) return;

            int key = job.ZipId;
            int jobId = job.JobId;

            using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
            {
                try
                {
                    Debug.WriteLine("Should be recieving");
                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = "citizen.science.executable.storage",
                        Key = key.ToString() + ".zip"
                    };
                    using (GetObjectResponse response = client.GetObject(request)) {
                        string root = HttpContext.Current.Server.MapPath("~/App_Data/Jobs/"+jobId); //Specify here where to save executables
                        string dest = Path.Combine(root, key.ToString() + ".zip");
                        if (!File.Exists(dest))
                        {
                            response.WriteResponseStreamToFile(dest);
                        }
                        try
                        {
                            String zipPath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/Jobs/" + jobId + "/" + key + ".zip");
                            String extractPath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/Jobs/" + jobId + "/Extracted");
                            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
                        }
                        catch
                        {
                            Debug.WriteLine("Executable already exists on server");
                        }
                    }

                    // Store the "work" for the job
                    string path = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/Jobs/" + jobId + "/work.txt");
                    string[] images = new string[job.Work.Length * 2];

                    for (int i = 0; i < job.Work.Length; i++) {
                        Work w = job.Work[i].Image1;
                        Work w2 = job.Work[i].Image2;

                        images[i * 2] = w.Key + " " + w.Bucket;
                        images[i * 2 + 1] = w2.Key + " " + w2.Bucket;
                    }

                    System.IO.File.WriteAllLines(path, images);
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    AWS.AWSerror(amazonS3Exception);
                }
            }
        }
    }
}
