using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System;
using Zip.InstallmentsService.API.Helper;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Zip.InstallmentsService.Entity.Dto;
using System.Net.Http;

namespace Zip.InstallmentsService.API.Middleware
{
    /// <summary>
    /// Global or common Error Handler Middleware (Custom Middleware) which includes logging and exception handling logic
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }

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
            var errorDetails = new ErrorDetailDto()
            {
                StatusCode = response.StatusCode,
                Message = error?.Message  //"Internal Server Error from the custom middleware."
            };
            var result = JsonSerializer.Serialize(errorDetails);
            await response.WriteAsync(result);
        }



    }

}
