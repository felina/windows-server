﻿using JobServer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.IO;
using JobServer.Controllers;
using JobServer.App_Code;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobServer.Executables
{
    public class ProcessManager
    {
        /// <summary>
        /// Dictionary (map) of loaded jobs 
        /// </summary>
        private static Dictionary<int, StoredJob> Jobs = new Dictionary<int, StoredJob>();

        // Adds a new job
        public static void AddJob(Job job)
        {
            if (JobLoaded(job.JobId))
            {
                Debug.WriteLine("Warning: Job ID " + job.JobId + " already exists (ProcessManager.AddJob()). Job was not added");
            }
            else
            {
                //Save the files to the windows server, some error checking. Make sure that Jobs is alway uptodate with what is actually
                // on the server    
                CreateJobController.CacheJob(job);
                
                // Load the job info into memory
                Jobs[job.JobId] = new StoredJob(job);
            }
        }

        // Returns a stored job
        public static StoredJob GetJob(int id)
        {
            if (JobLoaded(id))
            {
                return Jobs[id];
            }
            else if (JobCached(id))
            {
                Debug.WriteLine("Loading cached job");
                Job job = new Job();
                job.JobId = id;

                // Get zipId from stored zip
                string[] zips = Directory.GetFiles(HttpRuntime.AppDomainAppPath + "/App_Data/Jobs/" + id, "*.zip");
                string file = Path.GetFileNameWithoutExtension(zips[0]);
                job.ZipId = int.Parse(file);

                // Get images to work on
                string[] images = System.IO.File.ReadAllLines(HttpRuntime.AppDomainAppPath + "/App_Data/Jobs/" + id + "/work.txt");
                job.Work = new WorkArray[images.Length / 2];

                Work w = new Work();
                Work w2 = new Work();

                for (int i = 0; i < images.Length; i += 2)
                {
                    string[] l1 = images[i].Split(' ');
                    string[] l2 = images[i + 1].Split(' ');

                    w.Key = l1[0];
                    w.Bucket = l1[1];

                    w2.Key = l2[0];
                    w2.Bucket = l2[0];

                    job.Work[i] = new WorkArray();

                    job.Work[i / 2].Image1 = w;
                    job.Work[i / 2].Image2 = w2;
                }

                Jobs[id] = new StoredJob(job);
                return Jobs[id];
            }
            else
            {
                Debug.WriteLine("Warning: Job ID " + id + " could not be found (ProcessManager.GetJob()). Returning null");
                return null;
            }
        }

        // Tests if a job is loaded into memory
        public static bool JobLoaded(int id)
        {
            return Jobs.ContainsKey(id);
        }

        // Tests whether a job of given id is already stored 
        public static bool JobCached(int id)
        {
            // Checks server hard drive for the executable
            return System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/App_Data/Jobs/"+id));
        }


        // Launches the application on the command line and saves the results
        public static void RunJob(string fileName, int jobId)
        {
            
            

            // Change the number to allow more simultaneously running executables
            //if (JobQueue.RunningTasks < 4)
            //{
                // Threading of tasks
            JobQueue.AddToQueue(fileName, jobId);
        }
    }
}