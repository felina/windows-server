using System;
using System.Collections.Generic;

namespace JobServer.App_Code
{
    /// <summary>
    /// Static class which manages a Dictionary mapping Jobs to their work Queues.
    /// </summary>
    public class GlobalQueue
    {
        /// <summary>
        /// <para>The Global Dictionary which maps the JobID to a Queue of image tuples - the job's work queue.</para>
        /// <para>This allows us to download images in parallel with running the executable.</para>
        /// </summary>
        private static Dictionary<int, Queue<Tuple<string, string>>> DictQueue;

        /// <summary>
        /// Initialization method - creates the global dictionary queue.
        /// </summary>
        static GlobalQueue()
        {
            DictQueue = new Dictionary<int, Queue<Tuple<string, string>>>();
        }

        /// <summary>
        /// Adds values to the queue depending on their JobID.
        /// </summary>
        /// <param name="jobId">ID of the Job to add.</param>
        /// <param name="Image1">First image to add.</param>
        /// <param name="Image2">Second image to add.</param>
        public static void AddToQueue(int jobId, string Image1, string Image2)
        {
            if (!DictQueue.ContainsKey(jobId))
            {
                DictQueue.Add(jobId, new Queue<Tuple<string, string>>());
            }
            Queue<Tuple<string, string>> value;
            DictQueue.TryGetValue(jobId, out value);
            value.Enqueue(new Tuple<string, string>(Image1, Image2));
        }

        /// <summary>
        /// Returns the size of the work queue for a given job.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <returns>Size of job's queue.</returns>
        public static int QueueSize(int jobId)
        {
            if (!DictQueue.ContainsKey(jobId))
            {
                return 0;
            }
            Queue<Tuple<string, string>> value;
            DictQueue.TryGetValue(jobId, out value);
            return value.Count;
        }

        // Removes a vlue from a queue depending on the jobId. Once taken 
        // off the queue the images are used as command line arguments to the
        // executable.
        /// <summary>
        /// <para>Removes (pops) a work item from the Queue for the given job.</para>
        /// <para>The image tuple is returned for use as command line arguments to the executable.</para>
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <returns>The next work item for the job (image tuple).</returns>
        public static Tuple<string, string> RemoveFromQueue(int jobId)
        {
            if (!DictQueue.ContainsKey(jobId))
            {
                return null;
            }
            Queue<Tuple<string, string>> value;
            DictQueue.TryGetValue(jobId, out value);
            Tuple<string, string> result = value.Dequeue();
            return result;
        }
    }
}