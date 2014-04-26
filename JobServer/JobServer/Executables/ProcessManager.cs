using JobServer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using JobServer.Controllers;
using JobServer.App_Code;
using System.Text;
using System.IO;
using System.Threading;

namespace JobServer.Executables
{
    public class ProcessManager
    {
        /// <summary>
        /// Dictionary (map) of loaded jobs 
        /// </summary>
        private static Dictionary<int, StoredJob> Jobs = new Dictionary<int, StoredJob>();

        // Adds a new job
        public static void AddJob(StoredJob job)
        {
            if (JobExists(job.JobId))
            {
                Debug.WriteLine("Warning: Job ID " + job.JobId + " already exists (ProcessManager.AddJob()). Job was not added");
            }
            else
            {
                //Save the files to the windows server, some error checking. Make sure that Jobs is alway uptodate with what is actually
                // on the server    
                CreateJobController.AllocateExecutables(job.ZipId, job.JobId);
                
                //Temporary, wil fix soon, need to get multi-threading working
                //ImageDownload.Download(job, 100);
                Jobs[job.JobId] = job;
            }
        }

        // Returns a stored job
        public static StoredJob GetJob(int id)
        {
            if (JobExists(id))
            {
                return Jobs[id];
            }
            else
            {
                Debug.WriteLine("Warning: Job ID " + id + " could not be found (ProcessManager.GetJob()). Returning null");
                return null;
            }
        }

        // Tests whether a job of given id is already stored
        public static bool JobExists(int id)
        {
            //return Jobs.ContainsKey(id);
            // Checks server to see file exists on server, better to check the server to see if file exists
            // just incase server crashes and dictionary is reset
            return System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/App_Data/Jobs/" + id));
        }



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

        // Launches the application on the command line and saves the results
        public static void RunJob(string fileName, int jobId)
        {
            
            StoredJob job = GetJob(jobId);

            // Working towards threading
            Thread downloadImages = new Thread(new ThreadStart(() => ImageDownload.Download(job, 100))); //MAKE SURE TO GET RID OF HARDCODE
            downloadImages.Start();



            
            //StoredJob job = GetJob(jobId);
            WorkArray[] Images = job.Images;
            string filePath = HttpContext.Current.Server.MapPath("~/App_Data/Jobs/" + jobId + "/Extracted/" + fileName);
            string output = HttpContext.Current.Server.MapPath("~/App_Data/Jobs/" + jobId + "/results.csv");

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
                    // Generate the image arguments
                    startInfo.Arguments = Images[i].Image1.Key + " " + Images[i].Image2.Key;
                                       
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
                ResultUpload.AWSUpload(output, "citizen.science.image.storage.public", "5"); //Need to give upload name, currently hardcoded
            }
            catch
            {
                //Log error
            }
        }
    }
}