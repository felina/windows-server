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

namespace JobServer.App_Start
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

        public static void ListBucketUrls(string key, bool priv)
        {
            if (checkRequiredFields())
            {
                using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
                {
                    GetUrl(key, priv);
                }
            }
        }

        //Gets the url of a given bucket
        static void GetUrl(string key, bool priv)
        {
            try
            {
                var p = new GetPreSignedUrlRequest();
                p.BucketName = priv ? "citizen.science.image.storage" : "citizen.science.image.storage.public";
                p.Key = key;
                p.Expires = DateTime.Now.AddHours(1);
                Debug.WriteLine(client.GetPreSignedURL(p));
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
                    Debug.WriteLine("An Error, number {0}, occurred when listing buckets with the message '{1}", amazonS3Exception.ErrorCode, amazonS3Exception.Message);
                }
            }
        }


        // Saves an object from the server to a file (currently to App_Data)
        public static void GetObject(String key, bool priv)
        {
            using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
            {
                try
                {
                    GetObjectRequest request = new GetObjectRequest()
                    {
                        BucketName = priv ? "citizen.science.image.storage" : "citizen.science.image.storage.public",
                        Key = key
                    };

                    using (GetObjectResponse response = client.GetObject(request))
                    {
                        string title = response.Metadata["x-amz-meta-title"];
                        Console.WriteLine("The object's title is {0}", title);
                        string root = HttpContext.Current.Server.MapPath("~/App_Data");
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