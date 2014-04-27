using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobServer.Models
{
    /// <summary>
    /// Describes an image which can be retrieved from AWS
    /// </summary>
    public class Work
    {
        public string Key { get; set; }
        public string Bucket { get; set; }
    }
}