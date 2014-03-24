using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JobServer.Controllers
{
    public class UploadController : ApiController
    {
        // GET api/upload
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/upload/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/upload
        public void Post([FromBody]string value)
        {
        }

        // PUT api/upload/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/upload/5
        public void Delete(int id)
        {
        }
    }
}
