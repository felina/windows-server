using System;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace JobServer.App_Code
{
    /// <summary>
    /// Static class which provides AWS upload functionality for the Job Results.
    /// </summary>
    public static class ResultUpload
    {
        /// <summary>
        /// Uploads a CSV results file to shared AWS storage, allowing access to the results by other system components.
        /// </summary>
        /// <param name="filePath">Path of the file to upload.</param>
        /// <param name="bucketName">Bucket to upload to.</param>
        /// <param name="keyName">Storage key - used for retrieval. ".csv" is appended to this.</param>
        public static void AWSUpload(string filePath, string bucketName, string keyName)
        {
            try
            {
                TransferUtility fileTransferUtility = new TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.EUWest1));
                fileTransferUtility.Upload(filePath, bucketName, keyName + ".csv");
                Console.WriteLine("Upload Completed");
                UploadQueue.Upload();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                AWS.AWSerror(amazonS3Exception);
            }
        }
    }
}