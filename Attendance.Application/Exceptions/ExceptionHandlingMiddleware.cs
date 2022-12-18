using Attendance.Application.Common;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using Error = Attendance.Application.Common.Error;

namespace Attendance.Application.Exceptions
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ApiResponse _response;
        //private readonly ILog _logger;

        public ExceptionHandlingMiddleware()
        {
            _response = new ApiResponse();
            // _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundException ex)
            {
                await WriteAsync(context, (int)HttpStatusCode.NotFound, ex.Message, ex.StackTrace);
            }
            catch (ValidationException ex)
            {
                await WriteAsync(context, (int)HttpStatusCode.BadRequest, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Writes given text to the response body along with status code.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task WriteAsync(HttpContext context, int statusCode, string message, string stackTrace)
        {
            // _logger.Error($"Something went wrong. StatusCode = {statusCode} ErrorMessage = {message} StackTrace = {stackTrace}");
            // TODO: Might need complete exception object for better tracing of errors
            _response.Errors = new Error() { Message = message };
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(_response));
        }
    }
}
