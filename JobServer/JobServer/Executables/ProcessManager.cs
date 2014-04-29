using JobServer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.IO;
using JobServer.Controllers;
using JobServer.App_Code;

namespace JobServer.Executables
{
    /// <summary>
    /// Manages Jobs in the server memory.
    /// Also responsible for running the Job executables and storing their results.
    /// </summary>
    public class ProcessManager
    {
        /// <summary>
        /// Dictionary (map) of loaded jobs 
        /// </summary>
        private static Dictionary<int, StoredJob> Jobs = new Dictionary<int, StoredJob>();

        /// <summary>
        /// Utility function
        /// </summary>
        /// <param name="id">ID of job</param>
        /// <returns>The path to the given job</returns>
        private static string JobPath(int id)
        {
            return HttpRuntime.AppDomainAppPath + "/App_Data/Jobs/" + id;
        }

        /// <summary>
        /// Adds a new job - to both the cache and application memory
        /// </summary>
        /// <param name="job">Job Model object to store</param>
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

        /// <summary>
        /// Removes a job from memory and the cache. Aborts if the job is running, or if it isn't cached.
        /// If the job is queued, the execution will be aborted when it reaches the front of the queue.
        /// </summary>
        /// <param name="id">ID of job to remove</param>
        /// <returns>Whether the job was removed</returns>
        public static bool RemoveJob(int id)
        {
            bool cached = JobCached(id);

            if (!cached)
            {
                Debug.WriteLine("Warning: Attempted to remove job which is not in the cache (" + id + ")");
                return false;
            }

            // Remove from memory
            if (JobLoaded(id))
            {
                StoredJob job = GetJob(id);

                // Check it's not running
                if (job.Started && !job.Completed)
                {
                    return false;
                }

                Jobs.Remove(id);
            }

            // Remove from cache
            try
            {
                Directory.Delete(JobPath(id), true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieves a stored job. Loads the job from the cache if not in memory already.
        /// </summary>
        /// <param name="id">ID of job to load</param>
        /// <returns>The StoredJob object for the job</returns>
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
                string[] zips = Directory.GetFiles(JobPath(id), "*.zip");
                string file = Path.GetFileNameWithoutExtension(zips[0]);
                job.ZipId = int.Parse(file);

                // Get images to work on
                string[] images = System.IO.File.ReadAllLines(JobPath(id) + "/work.txt");
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

        /// <summary>
        /// Tests if a job is loaded into memory
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <returns>Whether the job exists in the Job dictionary</returns>
        public static bool JobLoaded(int id)
        {
            return Jobs.ContainsKey(id);
        }

        /// <summary>
        /// Tests whether a job is cached on the hard drive, independent of whether it is loaded in memory
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <returns>Whether the job exists on the hard drive</returns>
        public static bool JobCached(int id)
        {
            // Checks server hard drive for the executable
            return System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/App_Data/Jobs/"+id));
        }
    }
}