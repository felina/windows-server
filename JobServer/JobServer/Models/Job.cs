using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobServer.Models
{
    public class Job
    {
        public int JobId { get; set; }
        public int ZipId { get; set; }
        public string[] Work { get; set; }
    }
}