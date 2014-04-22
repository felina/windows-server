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
using JobServer.App_Start;
using System.Web;
using System.IO;

namespace JobServer.Controllers
{
    public class CreateJobController : ApiController
    {

        static IAmazonS3 client;

        // POST api/createjob
        public IHttpActionResult Post([FromBody]Job value)
        {       
            // Check our input
            if (value == null)
            {
                Debug.WriteLine("CreateJob POST: Nothing recieved");
                return BadRequest("Invalid or missing job definition");
            }

            // Check for existing job
            if (!ProcessManager.JobCached(value.JobId))
            {
                Debug.WriteLine("Caching new Job");
                CacheJob(value);
                ProcessManager.AddJob(new StoredJob(value));
                Debug.WriteLine("Job stored");
                return Ok("New job " + value.JobId + " stored");
            }
            else return BadRequest("Job already cached");
        }


        //Get zip file from S3, then store and extract it in the App_Data/Jobs directory under the jobId
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
                    System.IO.File.WriteAllLines(path, job.Work);
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                    {
                        Debug.WriteLine("Please check the provided AWS Credentials.");
                        Debug.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                    }
                    else
                    {
                        Debug.WriteLine("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message);
                    }
                }
            }
        }
    }
}
