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
            if (!ProcessManager.JobExists(value.JobId))
            {
                Debug.WriteLine("Storing new Job");
                ProcessManager.AddJob(new StoredJob(value));
                return Ok("New job " + value.JobId + " stored");
            }
            else return BadRequest("Job already exists");
        }

        public static void AllocateExecutables(String key, int jobId)
        {
            if (AWS.checkRequiredFields())
            {
                using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
                {
                    RetrieveExecutables(key, jobId);
                }
            }
        }



        //Get zip files from S3
        public static void RetrieveExecutables(String key, int jobId)
        {
            using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
            {
                try
                {
                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = "citizen.science.executable.storage",
                        Key = key
                    };
                    using (GetObjectResponse response = client.GetObject(request)) {
                        string title = response.Metadata["x-amz-meta-title"];
                        Console.WriteLine("The object's title is {0}", title);
                        string root = HttpContext.Current.Server.MapPath("~/App_Data/Job/" + jobId); //Specify here where to save executables
                        string dest = Path.Combine(root, key);
                        if (!File.Exists(dest))
                        {
                            response.WriteResponseStreamToFile(dest);
                        }
                    }
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
