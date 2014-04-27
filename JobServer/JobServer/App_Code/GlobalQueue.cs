using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobServer.App_Code
{
    public class GlobalQueue
    {
        
        private static Dictionary<int, Queue<Tuple<string, string>>> DictQueue;

        static GlobalQueue()
        {
            DictQueue = new Dictionary<int, Queue<Tuple<string, string>>>();
        }

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