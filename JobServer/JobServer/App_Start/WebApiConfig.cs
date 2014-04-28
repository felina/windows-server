using System.Net.Http.Headers;
using System.Web.Http;

namespace JobServer
{
    /// <summary>
    /// Static configuration class.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers Http configuration for the server instance.
        /// </summary>
        /// <param name="config">Existing Http Configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // API parameter formatters
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
