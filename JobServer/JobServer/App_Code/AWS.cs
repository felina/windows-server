using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

namespace JobServer.App_Code
{
    public class AWS
    {
        static IAmazonS3 client;

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


        //Standard Error
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


        public static void ListBucketUrls(string key, string bucket)
        {
            if (checkRequiredFields())
            {
                using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
                {
                    GetUrl(key, bucket);
                }
            }
        }

        //Gets the url of a given bucket
        static void GetUrl(string key, string bucket)
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


        // Saves an object from the server to a file (currently to App_Data)
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
                        Console.WriteLine("The object's title is {0}", title);
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