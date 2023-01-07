using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Zip.InstallmentsService.API.Helper;
using Zip.InstallmentsService.Entity.Common;

namespace Zip.InstallmentsService.API.Middleware
{
    /// <summary>
    /// Global or common Error Handler Middleware (Custom Middleware) which includes logging and exception handling logic
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        /// <summary>
        /// Intialization in Constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }

        /// <summary>
        /// Method which is invoked and makes call to core method to handle exception in case of any errpor
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                _logger.LogError($"Something went wrong: {error}", error.Message);
                await HandleExceptionAsync(context, error);
            }
        }

        /// <summary>
        /// Core Logic to handle exceptions
        /// </summary>
        /// <param name="context"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception error)
        {
            
            var response = context.Response;
            response.ContentType = "application/json";

            //identify and set status code
            switch (error)
            {
                case AppException e:
                    // custom application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException e:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            //prepare object and return error details
            var errorDetails = new ErrorDetail()
            {
                StatusCode = response.StatusCode,
                Message = error?.Message  //"Internal Server Error from the custom middleware."
            };
            var result = JsonSerializer.Serialize(errorDetails);
            await response.WriteAsync(result);
        }



    }

}
