﻿using JobServer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

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
            return Jobs.ContainsKey(id);
        }

        // Runs a job on the command line
        public static void RunJob(string fileName, int id)
        {
            LaunchCommandLineApp(fileName, id);
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
        static void LaunchCommandLineApp(string fileName, int jobId)
        {
            StoredJob job = GetJob(jobId);
            string[] Images = job.Images;

            // Validate each image
            foreach (string img in Images)
            {
                if (!ValidateImageName(img))
                {
                    Debug.WriteLine("Invalid image hash " + img + ". Aborting executable launch.");
                    return;
                }
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = false;

            // Generate the image arguments
            string args = "";
            foreach (string img in Images)
            {
                args = args + img + " ";
            }

            startInfo.Arguments = args;

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    string strOut = exeProcess.StandardOutput.ReadToEnd();
                    string strErr = exeProcess.StandardError.ReadToEnd();
                    exeProcess.WaitForExit();

                    // Save the results
                    job.Result = strOut;
                    job.Errors = strErr;
                    job.ExitCode = exeProcess.ExitCode;
                }
            }
            catch
            {
                //Log error
            }
        }
    }
}