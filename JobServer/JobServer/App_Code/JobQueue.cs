using JobServer.Executables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace JobServer.App_Code
{
    /// <summary>
    /// Static class which manages the execution queue for jobs.
    /// </summary>
    public static class JobQueue
    {
        /// <summary>
        /// The queue storing jobs which will be run.
        /// </summary>
        private static Queue<Tuple<string,int>> TaskQueue;
        
        /// <summary>
        /// Gives the number of tasks currently running on the server.
        /// </summary>
        public static int RunningTasks;

        /// <summary>
        /// Maximum number of executables to be run at once.
        /// </summary>
        private static int ExecutableLimit;

        /// <summary>
        /// <para>Initialization method - creates the queue and sets the executable limit. Call before using this class's methods.</para>
        /// <para>Modify this to change the executable limit.</para>
        /// </summary>
        static JobQueue()
        {
            TaskQueue = new Queue<Tuple<string,int>>();
            RunningTasks = 0;
            ExecutableLimit = 4; //Choose number for number of simultaneously running executables
        }

        /// <summary>
        /// Adds a job to the queue.
        /// </summary>
        /// <param name="fileName">The name of the main executable in the job's .zip.</param>
        /// <param name="jobId">The Job's ID.</param>
        /// <remarks>Possibly check for running instances of the same JobID?</remarks>
        public static void AddToQueue(string fileName, int jobId)
        {
            TaskQueue.Enqueue(new Tuple<string, int>(fileName, jobId));
            AllocateJobs();
        }

        /// <summary>
        /// <para>Allocates jobs from the queue to parallel execution threads.</para>
        /// <para>Jobs will be allocated up to the set execution limit, and are dequeued when their execution starts.</para>
        /// </summary>
        public static void AllocateJobs()
        {
            for (int i = 0; i < (ExecutableLimit - RunningTasks) && RunningTasks < ExecutableLimit && TaskQueue.Count != 0; i++)
            {
                if (RunningTasks < ExecutableLimit)
                {
                    Tuple<string, int> values = RemoveFromQueue();
                    string fileName = values.Item1;
                    int jobId = values.Item2;
                    StoredJob job = ProcessManager.GetJob(jobId);
                    if (job == null || job.Stopped) //Don't run job if stopped externally.
                    {
                        // Because job is stopped don't run the job
                    }
                    else
                    {
                        RunningTasks += 1;
                        Task.Factory.StartNew(() => new ImageDownload().Download(job));
                        Task.Factory.StartNew(() => new StartTask().RunTask(fileName, jobId));
                    }
                }
            }  
        }

        /// <summary>
        /// Get the number of jobs in the queue.
        /// </summary>
        /// <returns>Size of the queue.</returns>
        public static int QueueSize()
        {
            return TaskQueue.Count;
        }

        /// <summary>
        /// <para>Removes (pops) the first job in the queue, reducing its size by one.</para>
        /// <para>This returns the (command, JobID) tuple stored in the queue.</para>
        /// </summary>
        /// <returns>(command, JobID) tuple which was removed.</returns>
        public static Tuple<string, int> RemoveFromQueue()
        {
            return TaskQueue.Dequeue();
        }

        /// <summary>
        /// Returns the jobs currently left on the queue as an array of JobIDs.
        /// </summary>
        /// <returns>Array of JobIDs left in the queue</returns>
        public static int[] LeftOnQueue()
        {
            Tuple<string,int>[] values = TaskQueue.ToArray();
            int[] results = new int[TaskQueue.Count];
            for (int i = 0; i < TaskQueue.Count; i++)
            {
                results[i] = values[i].Item2;
            }
            return results;
        }

    }
}