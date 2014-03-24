using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace JobServer.Executables
{
    public class ProcessManager
    {
        public static void RunJob(String fileName)
        {
            LaunchCommandLineApp(fileName);
        }

        static void LaunchCommandLineApp(String fileName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.UseShellExecute = true;
            startInfo.Arguments = "4 3";

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                //Log error
            }
        }
    }
}