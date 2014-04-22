using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using JobServer.App_Code;
using JobServer.Executables;
using System.Diagnostics;

namespace JobServer.App_Code
{
    public class ImageDownload
    {
        public static void Download(StoredJob value, int limit)
        {
            // NEED TO CHECK THAT JOB ACTUALLY STORED
            for (int i = 0; i < value.Images.Length && i < limit; i++)
            {
                //Downloads the images from AWS and saves to directory in which job is stored
                AWS.GetObject(value.Images[i].Image1.Key, value.Images[i].Image1.Bucket, value.JobId);
                AWS.GetObject(value.Images[i].Image2.Key, value.Images[i].Image1.Bucket, value.JobId);
            }
        }
    }
}