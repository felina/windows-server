using System;
using System.Collections.Generic;

namespace JobServer.App_Code
{
    /// <summary>
    /// Static class which manages the upload queue.
    /// </summary>
    public class UploadQueue
    {
        /// <summary>
        /// The upload queue, holding (output, bucket, id) tuples.
        /// </summary>
        private static Queue<Tuple<string, string, string>> UpQueue;

        /// <summary>
        /// Indicates whether an upload is currently taking place.
        /// </summary>
        private static bool processing;

        /// <summary>
        /// Initialization method - creates the queue. Call before using this class's methods.
        /// </summary>
        static UploadQueue() 
        {
            UpQueue = new Queue<Tuple<string, string, string>>();
            processing = false;
        }

        /// <summary>
        /// Adds an upload job to the queue.
        /// </summary>
        /// <param name="output">Output file path.</param>
        /// <param name="bucket">AWS bucket to retrieve the item from.</param>
        /// <param name="id">Item key.</param>
        public static void AddToQueue(string output, string bucket, string id)
        {
            UpQueue.Enqueue(new Tuple<string, string, string>(output, bucket, id));
            Upload();
        }

        /// <summary>
        /// <para>Removes (pops) the first upload job in the queue, reducing its size by one.</para>
        /// <para>This returns the (output, bucket, id) tuple stored in the queue.</para>
        /// </summary>
        /// <returns>(output, bucket, id) tuple which was removed.</returns>
        public static Tuple<string, string, string> RemoveFromQueue()
        {
            return UpQueue.Dequeue();
        }

        /// <summary>
        /// Gets the number of upload jobs in the queue.
        /// </summary>
        /// <returns>The size of the queue.</returns>
        public static int QueueSize()
        {
            return UpQueue.Count;
        }

        /// <summary>
        /// <para>Attempts to complete the first upload job on the queue.</para>
        /// <para>Upload will commence as long as the queue is not empty and there is not another upload in progress.</para>
        /// </summary>
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