using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobServer.Models
{
    public class JobResult
    {
        public int JobId { get; set; }
        public string ImageA { get; set; }
        public string ImageB { get; set; }
        public int? ExitCode { get; set; }
        public string Result { get; set; }
        public string Errors { get; set; }
    }
}