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
        public static JobResult RunJob(String fileName, String ImageA, String ImageB)
        {
            return LaunchCommandLineApp(fileName, ImageA, ImageB);
        }

        static JobResult LaunchCommandLineApp(String fileName, String ImageA, String ImageB)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.UseShellExecute = false;
            startInfo.Arguments = "4 3";
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
                    return new JobResult{JobId = 1, ImageA = "0deae28a26727ebe30ecf2896e5862f1", ImageB = "2d1dac96639c5e6f6246f9315625ccbc", ExitCode = exeProcess.ExitCode, Result = a } ;
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