using JobServer.Executables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JobServer.App_Code
{
    public static class JobQueue
    {

        // Initialise a queue that will store jobs that need to be run
        private static Queue<Tuple<string,int>> TaskQueue;
        
        // Gives the current number of running jobs on the server
        public static int RunningTasks;

        // An int to represent the maximum number of jobs wanted running at once
        private static int ExecutableLimit;

        //Initialise all of the above
        static JobQueue()
        {
            TaskQueue = new Queue<Tuple<string,int>>();
            RunningTasks = 0;
            ExecutableLimit = 1; //Choose number for number of simultaneously running executables
        }
            
        //Possibly check for running instances of same jobId?
        public static void AddToQueue(string fileName, int jobId)
        {
            TaskQueue.Enqueue(new Tuple<string, int>(fileName, jobId));
            AllocateJobs();
        }

        //Starts to run jobs
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
                    Task.Factory.StartNew(() => new ImageDownload().Download(job, 100)); //Image download limit?
                    Task.Factory.StartNew(() => new StartTask().RunTask(fileName, jobId));
                    //Task.WaitAll(downloadImages, runningTasks);
                    Debug.WriteLine(RunningTasks);
                }
            }  
        }


        public static int QueueSize()
        {
            return TaskQueue.Count;
        }

        // Takes a job off of the queue 
        public static Tuple<string, int> RemoveFromQueue()
        {
            return TaskQueue.Dequeue();
        }

        // Returns an array of jobs that are remaining on the queue
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