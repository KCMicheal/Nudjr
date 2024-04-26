using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nudjr_AppCore.Services.Utility
{
    public class ServiceResponse<T> where T : class
    {
        /// <summary>
        /// Data returned from the public service method
        /// </summary>
        public T? Data { get; set; }
        /// <summary>
        /// Agreed Http Status Code to be sent in controller as a response
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        /// <summary>
        /// Message to be returned back to the API client.
        /// It could either be a failure message or a prompt to notify the api client about a successful operation being carried out
        /// </summary>
        public string? Message { get; set; }

    }

    public class ServiceResponse
    {
        /// <summary>
        /// Agreed Http Status Code to be sent in controller as a response
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        /// <summary>
        /// Message to be returned back to the API client.
        /// It could either be a failure message or a prompt to notify the api client about a successful operation being carried out
        /// </summary>
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
