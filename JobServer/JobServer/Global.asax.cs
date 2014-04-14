using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using JobServer.App_Start;
using System.Diagnostics;

namespace JobServer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //Writes to the output to say if the AWS entered are correct or not
            Debug.WriteLine(AWS.checkRequiredFields() ? "AWS CONFIRMED" : "AWS UNCONFIRMED");
            //AWS.GetObject("b90f6c7e57f955032456d1e9016cd4a2", true);
        }
    }
}
