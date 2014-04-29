namespace JobServer.Models
{
    /// <summary>
    /// Describes a generic query response, with jobId, res and message
    /// </summary>
    public class GenericResponse
    {
        public int? jobId { get; set; }
        public bool res { get; set; }
        public string message { get; set; }

        /// <summary>
        /// Creates a generic success response
        /// </summary>
        /// <param name="jobId">ID of queried job</param>
        /// <returns>New GenericResponse object</returns>
        public static GenericResponse Success(int jobId)
        {
            GenericResponse gr = new GenericResponse();
            gr.res = true;
            gr.jobId = jobId;

            return gr;
        }

        /// <summary>
        /// Creates a generic success response
        /// </summary>
        /// <returns>New GenericResponse object</returns>
        public static GenericResponse Success()
        {
            GenericResponse gr = new GenericResponse();
            gr.res = true;

            return gr;
        }

        /// <summary>
        /// Creates a generic failure response
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>New GenericResponse object</returns>
        public static GenericResponse Failure(string message)
        {
            GenericResponse gr = new GenericResponse();
            gr.res = false;
            gr.message = message;

            return gr;
        }
    }
}