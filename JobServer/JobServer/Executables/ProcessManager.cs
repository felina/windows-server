using JobServer.Models;
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
        public static Dictionary<int, StoredJob> Jobs = new Dictionary<int, StoredJob>();

        public static JobResult RunJob(String fileName, String ImageA, String ImageB)
        {
            return LaunchCommandLineApp(fileName, ImageA, ImageB);
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



        static JobResult LaunchCommandLineApp(String fileName, String ImageA, String ImageB)
        {

            if(!ValidateImageName(ImageA) || !ValidateImageName(ImageB)) {
                return null;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.UseShellExecute = false;
            startInfo.Arguments = ImageA + " " + ImageB;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardError = false;
            startInfo.CreateNoWindow = false;

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    string a = exeProcess.StandardOutput.ReadToEnd();
                    exeProcess.WaitForExit();
                    return new JobResult{JobId = 1, ImageA = ImageA, ImageB = ImageB, ExitCode = exeProcess.ExitCode, Result = a } ;
                }
            }
            catch
            {
                //Log error
            }
            return null;
        }
    }
}