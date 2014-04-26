using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobServer.Models
{
    /// <summary>
    /// A tuple of Work objects - two images to be processed
    /// </summary>
    public class WorkArray
    {
        public Work Image1 { get; set; }
        public Work Image2 { get; set; }
    }
}