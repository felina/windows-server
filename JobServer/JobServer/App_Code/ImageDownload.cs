﻿using JobServer.Executables;
using System;
using System.IO;
using System.Threading.Tasks;

namespace JobServer.App_Code
{
    /// <summary>
    /// Provides image download functionality for images on AWS.
    /// </summary>
    public class ImageDownload
    {
        /// <summary>
        /// Downloads the images for the specified job.
        /// </summary>
        /// <example>  
        /// This starts an image download task:
        /// <code> 
        /// Task.Factory.StartNew(() => new ImageDownload().Download(job, 100));
        /// </code> 
        /// </example> 
        /// <param name="value">The job whose images are to be fetched.</param>
        /// <param name="limit">Limit on number of images to download.</param>
        public void Download(StoredJob value)
        {
            // Path where images will be stored
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Jobs/" + value.JobId);
            
            // Check that Job is actually stored on server before starting to save images
            if (Directory.Exists(path))
            {
                string work = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Jobs/" + value.JobId);
                for (int i = 0; i < value.Images.Length; i++)
                {
                    //Downloads the images from AWS Concurrently and saves to directory in which job is stored
                    Task image1 = Task.Factory.StartNew(() => AWS.GetObject(value.Images[i].Image1.Key, value.Images[i].Image1.Bucket, value.JobId));
                    Task image2 = Task.Factory.StartNew(() => AWS.GetObject(value.Images[i].Image2.Key, value.Images[i].Image1.Bucket, value.JobId));
                    Task.WaitAll(image1, image2);

                    //Pushes the image onto the GlobalQueue
                    GlobalQueue.AddToQueue(value.JobId, value.Images[i].Image1.Key, value.Images[i].Image2.Key);
                }
                //FINISHED
            }
            else
            {
                // Log Error, possibly wait some time and try again
                Console.WriteLine("Job is not stored");
            }
        }
    }
}