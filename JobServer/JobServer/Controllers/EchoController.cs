using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JobServer.Controllers
{

    //NEED TO DELETE WHOLE CLASS BECAUSE ONLY USED FOR TESTING!!!!!!!!!!!!!!!!!!!!!!!!



    public class EchoController : ApiController
    {
        // GET api/echo
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/echo/5
        public string Get(int id)
        {
            return id.ToString();
        }
    }
}
