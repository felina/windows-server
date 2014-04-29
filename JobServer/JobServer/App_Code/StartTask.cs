using JobServer.Executables;
using JobServer.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace JobServer.App_Code
{
    /// <summary>
    /// Allows parallel execution of Jobs.
    /// </summary>
    public class StartTask
    {
        /// <summary>
        /// Static method - checks valididty of given image hash.
        /// </summary>
        /// <param name="Image">Hash to check.</param>
        /// <returns>Whether the hash is valid.</returns>
        static bool ValidateImageName(String Image)
        {
            if (Image.Length == 32)
            {
                for (int i = 0; i < Image.Length; i++)
                {
                    if (char.IsUpper(Image[i]) && (!(char.IsNumber(Image[i]))))
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Attempts to execute the given Job.</para>
        /// <para>Output is stored line-by-line in "results.csv" in the App_Data/{id} folder for the job.</para>
        /// </summary>
        /// <param name="fileName">Name of executable to run from the Job's .zip.</param>
        /// <param name="jobId">Job's ID.</param>
        public void RunTask(string fileName, int jobId)
        {
            StoredJob job = ProcessManager.GetJob(jobId);
            WorkArray[] Images = job.Images;
            string filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Jobs/" + jobId + "/Extracted/" + fileName);
            string output = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Jobs/" + jobId + "/results.csv");

            // Validate each image
            foreach (WorkArray img in Images)
            {
                if (!ValidateImageName(img.Image1.Key) || !ValidateImageName(img.Image2.Key))
                {
                    Debug.WriteLine("Invalid image hash " + img.Image1.Key + "or" + img.Image2.Key + ". Aborting executable launch.");
                    return;
                }
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filePath;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = false;

            //Set up header for file
            var w = new StreamWriter(output);
            var line = string.Format("{0},{1},{2},{3}", "Image1", "Image2", "Result", "Errors");
            w.WriteLine(line);
            w.Flush();

            try
            {
                for (int i = 0; i < Images.Length; i++)
                {
                    while (GlobalQueue.QueueSize(jobId) == 0 || job.Paused == true)
                    {
                        // Hang till there's stuff on the queue to process

                        //Add sleep or wait
                    }
                    if (job.Stopped)
                    {
                        if (i > 0) //If i == 0 then RunningTasks hasn't been increased yet
                        {
                            JobQueue.RunningTasks -= 1;
                        }
                        w.Close();
                        //Maybe clean job of system
                        return;
                    }
                    if (i == 0)
                    {
                        Debug.WriteLine("Job + " + job.JobId + " started");
                        job.Started = true;
                    }
                    job.BatchIndex = i; // Helps to give the progress of the code
                    Tuple<string, string> arguments = GlobalQueue.RemoveFromQueue(jobId);
                    // Generate the image arguments
                    startInfo.Arguments = arguments.Item1 + " " + arguments.Item2 + " ";

                    using (job.exeProcess = Process.Start(startInfo))
                    {
                        string strOut = job.exeProcess.StandardOutput.ReadToEnd();
                        string strErr = job.exeProcess.StandardError.ReadToEnd();
                        job.exeProcess.WaitForExit();

                        // Save the results
                        job.ExitCode = job.exeProcess.ExitCode;

                        //Write to csv files
                        string first = Images[i].Image1.Key;
                        string second = Images[i].Image2.Key;
                        //Check that there are no commas in strings otherwise cause error
                        String[] strOutArray = strOut.Split(',');
                        String[] strErrArray = strErr.Split(',');
                        strOut = String.Join("", strOutArray);
                        strErr = String.Join("", strErrArray);
                        line = string.Format("{0},{1},{2},{3}", first, second, strOut, strErr).Trim();
                        w.WriteLine(line);
                        w.Flush();
                    }
                }
                w.Close();
                job.Completed = true; //Signifies that the job is now complete
                //Given upload destination is currently the jobId
                JobQueue.RunningTasks -= 1; // Frees up space for the next running executable
                Action b = delegate { UploadQueue.AddToQueue(output, "citizen.science.image.storage.public", job.JobId.ToString()); };
                b();
                Debug.WriteLine("Uploaded:" + jobId);
                JobQueue.AllocateJobs();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Job execution failed: " + e.Message);
            }
        }
    }
}