using JobServer.Executables;
using JobServer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace JobServer.App_Code
{
    public class StartTask
    {

        //Validates the image to make sure it's an MD-5 Hash
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
            var line = string.Format("{0},{1},{2}", "Image1", "Image2", "Result");
            w.WriteLine(line);
            w.Flush();

            try
            {
                for (int i = 0; i < Images.Length; i++)
                {
                    while (GlobalQueue.QueueSize(jobId) == 0)
                    {
                        // Hang till there's stuff on the queue to process
                    }
                    if (i == 0)
                    {
                        job.Started = true;
                        JobQueue.RunningTasks += 1;
                    }
                    job.BatchIndex = i; // Helps to give the progress of the code
                    Tuple<string, string> arguments = GlobalQueue.RemoveFromQueue(jobId);
                    // Generate the image arguments
                    startInfo.Arguments = arguments.Item1 + " " + arguments.Item2 + " ";

                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        string strOut = exeProcess.StandardOutput.ReadToEnd();
                        string strErr = exeProcess.StandardError.ReadToEnd();
                        exeProcess.WaitForExit();

                        // Save the results
                        job.Result = strOut;
                        job.Errors = strErr;
                        job.ExitCode = exeProcess.ExitCode;

                        //Write to csv files
                        var first = Images[i].Image1.Key;
                        var second = Images[i].Image2.Key;
                        line = string.Format("{0},{1},{2}", first, second, job.Result).Trim();
                        w.WriteLine(line);
                        w.Flush();
                    }
                }
                w.Close();
                job.Completed = true; //Sifnifies that the job is now complete
                //Given upload destination is currently the jobId
                JobQueue.RunningTasks -= 1; // Frees up space for the next running executable
               
                //Debugging
                for (int i = 0; i < JobQueue.LeftOnQueue().Length; i++)
                    //{
                    //    Debug.WriteLine(JobQueue.LeftOnQueue()[i].ToString());
                    //}
                JobQueue.AllocateJobs();
                ResultUpload.AWSUpload(output, "citizen.science.image.storage.public", job.JobId.ToString());
            }
            catch
            {
                //Log error
            }
        }
    }
}