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
            if (!ProcessManager.JobExists(value.JobId))
            {
                Debug.WriteLine("Storing new Job");
                ProcessManager.AddJob(new StoredJob(value));
                ProcessManager.RunJob("TestExecutable", value.JobId);
                return Ok("New job " + value.JobId + " stored");
            }
            else return BadRequest("Job already exists");

        }

        // Checks the AWS credentials and then saves the files from AWS onto the windows server
        public static void AllocateExecutables(int key, int jobId)
        {
            if (AWS.checkRequiredFields())
            {
                using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
                {
                    Debug.WriteLine("Should be recieving");
                    RetrieveExecutables(key, jobId);
                }
            }
        }



        //Get zip files from S3, then put them into the App_Data/Jobs directory under there jobId and then unzip them
        public static void RetrieveExecutables(int key, int jobId)
        {
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
                        string dest = Path.Combine(root, key.ToString());
                        if (!File.Exists(dest))
                        {
                            response.WriteResponseStreamToFile(dest);
                        }
                        try
                        {
                            String zipPath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/Jobs/" + jobId + "/" + key);
                            String extractPath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data/Jobs/" + jobId + "/Extracted");
                            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
                        }
                        catch
                        {
                            Debug.WriteLine("Executable already exists on server");
                        }
                    }
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    AWS.AWSerror(amazonS3Exception);
                }
            }
        }
    }
}
