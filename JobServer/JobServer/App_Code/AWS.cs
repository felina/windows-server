using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace JobServer.App_Code
{
    /// <summary>
    /// Static class providing methods to query and access Amazon Web Service storage for the system.
    /// </summary>
    public class AWS
    {
        /// <summary>
        /// The S3 client object.
        /// </summary>
        static IAmazonS3 client;

        /// <summary>
        /// Checks that the AWS keys are stored in the configuration settings.
        /// </summary>
        /// <returns>Whether or not the keys are present.</returns>
        public static bool checkRequiredFields()
        {
            NameValueCollection appConfig = ConfigurationManager.AppSettings;

            if (string.IsNullOrEmpty(appConfig["AWSAccessKey"]))
            {
                Debug.WriteLine("AWSAccessKey was not set in the App.config file.");
                return false;
            }
            if (string.IsNullOrEmpty(appConfig["AWSSecretKey"]))
            {
                Debug.WriteLine("AWSSecretKey was not set in the App.config file.");
                return false;
            }
            if (string.IsNullOrEmpty(appConfig["AWSRegion"]))
            {
                Debug.WriteLine("The variable AWSRegion is not set.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Standardized error handler for S3 exceptions - provides debug output.
        /// </summary>
        /// <param name="amazonS3Exception">Exception to handle.</param>
        public static void AWSerror(AmazonS3Exception amazonS3Exception)
        {
            if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
            {
                Debug.WriteLine("Please check the provided AWS Credentials.");
            }
            else
            {
                Debug.WriteLine("An Error, number {0}, occurred when listing buckets with the message '{1}", amazonS3Exception.ErrorCode, amazonS3Exception.Message);
            }
        }

        /// <summary>
        /// Gets the url of the given item & bucket. This is printed to debug output.
        /// </summary>
        /// <param name="key">Key for the item.</param>
        /// <param name="bucket">Bucket the item is stored in.</param>
        static void GetUrl(string key, string bucket)
        {
            if (checkRequiredFields())
            {
                using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
                {
                    try
                    {
                        var p = new GetPreSignedUrlRequest();
                        p.BucketName = bucket;
                        p.Key = key;
                        p.Expires = DateTime.Now.AddHours(1);
                        Debug.WriteLine(client.GetPreSignedURL(p));
                    }
                    catch (AmazonS3Exception amazonS3Exception)
                    {
                        AWSerror(amazonS3Exception);
                    }
                }
            }
        }

        // Saves an object from the server to a file (currently to App_Data)
        /// <summary>
        /// Fetches an item from AWS storage and stores it locally, in App_Data.
        /// </summary>
        /// <param name="key">Key of item to fetch.</param>
        /// <param name="bucket">Bucket to fetch from.</param>
        /// <param name="id">Job ID - indicates folder to store the response in.</param>
        public static void GetObject(string key, string bucket, int id)
        {
            using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
            {
                try
                {
                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = bucket,
                        Key = key
                    };
                    using (GetObjectResponse response = client.GetObject(request))
                    {
                        string title = response.Metadata["x-amz-meta-title"];
                        //Console.WriteLine("The object's title is {0}", title);
                        string root = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Jobs/" + id + "/Images");
                        string dest = Path.Combine(root, key);
                        if (!File.Exists(dest))
                        {
                            response.WriteResponseStreamToFile(dest);
                        }
                    }
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    AWSerror(amazonS3Exception);
                }
            }
        }
    }
}