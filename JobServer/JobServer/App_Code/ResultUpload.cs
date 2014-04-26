using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.IO;


namespace JobServer.App_Code
{
    public class ResultUpload
    {
        // Uploads a csv file to AWS
        public static void AWSUpload(string filePath, string bucketName, string keyName)
        {
            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.EUWest1));
                fileTransferUtility.Upload(filePath, bucketName, keyName + ".csv");
                Console.WriteLine("Upload Completed");
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                AWS.AWSerror(amazonS3Exception);
            }
        }
    }
}