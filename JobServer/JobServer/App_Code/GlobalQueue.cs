using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobServer.App_Code
{
    public class GlobalQueue
    {
        //The Global Dictionary which maps the jobId to a Queue of images. This allows 
        //us to download images in parallel with running the executable.
        private static Dictionary<int, Queue<Tuple<string, string>>> DictQueue;

        //Initialise the DictQueue
        static GlobalQueue()
        {
            DictQueue = new Dictionary<int, Queue<Tuple<string, string>>>();
        }

        //Add values onto the queue depending on the jobId.
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

        //Returns the size of a given queue depending on its jobId
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