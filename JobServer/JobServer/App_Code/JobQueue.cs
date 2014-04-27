using JobServer.Executables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JobServer.App_Code
{
    public static class JobQueue
    {
        private static Queue<Tuple<string,int>> TaskQueue;
        public static int RunningTasks;
        private static int ExecutableLimit;

        static JobQueue()
        {
            TaskQueue = new Queue<Tuple<string,int>>();
            RunningTasks = 0;
            ExecutableLimit = 4; //Choose number for number of simultaneously running executables
        }

        //Possibly check for running instances of same jobId?
        public static void AddToQueue(string fileName, int jobId)
        {
            TaskQueue.Enqueue(new Tuple<string, int>(fileName, jobId));
            AllocateJobs();
        }


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
                    Task downloadImages = Task.Factory.StartNew(() => new ImageDownload().Download(job, 100));
                    Task runningTasks = Task.Factory.StartNew(() => new StartTask().RunTask(fileName, jobId));
                    Task.WaitAll(downloadImages, runningTasks);
                }
            }  
        }


        public static int QueueSize()
        {
            return TaskQueue.Count;
        }


        public static Tuple<string, int> RemoveFromQueue()
        {
            return TaskQueue.Dequeue();
        }

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