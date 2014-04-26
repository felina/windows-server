using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using JobServer.App_Code;
using JobServer.Executables;
using System.Diagnostics;
using System.IO;

namespace JobServer.App_Code
{
    public class ImageDownload
    {
        public static void Download(StoredJob value, int limit)
        {
            // Path where images will be stored
            string path = HttpContext.Current.Server.MapPath("~/App_Data/Jobs/" + value.JobId + "/Images");
            
            // NEED TO CHECK THAT JOB ACTUALLY STORED
            if (Directory.Exists(path))
            {
                for (int i = 0; i < value.Images.Length && i < limit; i++)
                {
                    //Downloads the images from AWS and saves to directory in which job is stored
                    AWS.GetObject(value.Images[i].Image1.Key, value.Images[i].Image1.Bucket, value.JobId);
                    AWS.GetObject(value.Images[i].Image2.Key, value.Images[i].Image1.Bucket, value.JobId);
                }
            }
            else
            {
                Console.WriteLine("Job is not stored");
            }
        }
    }
}