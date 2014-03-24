using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace JobServer.Controllers
{
    public class ZipUploadController : ApiController
    {
        public async Task<HttpResponseMessage> PostFormData()
        {
            // Check the content type
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            Debug.WriteLine("path: " + root);
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    Debug.WriteLine(file.Headers.ContentDisposition.FileName);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
