using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using JobServer.App_Code;

namespace JobServer.App_Code
{
    public class UploadQueue
    {
        private static Queue<Tuple<string, string, string>> UpQueue;
        private static bool processing;


        static UploadQueue() 
        {
            UpQueue = new Queue<Tuple<string, string, string>>();
            processing = false;
        }

        public static void AddToQueue(string output, string bucket, string id)
        {
            UpQueue.Enqueue(new Tuple<string, string, string>(output, bucket, id));
            Upload();
        }

        public static Tuple<string, string, string> RemoveFromQueue()
        {
            return UpQueue.Dequeue();
        }

        public static int QueueSize()
        {
            return UpQueue.Count;
        }

        public static void Upload()
        {
            if (QueueSize() != 0 && !processing)
            {
                processing = true;
                Tuple<string, string, string> values = RemoveFromQueue();
                ResultUpload.AWSUpload(values.Item1, values.Item2, values.Item3);
                processing = false;
            }
        }
    }
}