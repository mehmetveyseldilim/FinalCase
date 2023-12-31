using System.Net;
using Banking.Shared;
using Banking.Shared.Exceptions;

namespace Banking.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            _logger.LogInformation($"{nameof(GlobalExceptionHandlerMiddleware)} executing..");
            
            try
            {
                _logger.LogDebug($"Global exception calling next middleware");
                await _next(httpContext); // calling next middleware
            }

            catch(Exception ex)
            {

               _logger.LogDebug($"{nameof(GlobalExceptionHandlerMiddleware)} has been hit because of error.");

                httpContext.Response.ContentType = "application/json";
                HttpStatusCode httpStatusCode = IdentifyHttpStatusCode(ex);
 
                httpContext.Response.StatusCode = (int) httpStatusCode;

                _logger.LogError($"Something went wrong: {ex.Message}");

                await httpContext.Response.WriteAsync(new ErrorDetails()
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Message = new List<string> { ex.Message }
                }.ToString());         
            }
        }

        private  HttpStatusCode IdentifyHttpStatusCode(Exception ex) => ex switch
        {
            BadRequestException => HttpStatusCode.BadRequest,
            NotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };
    }
}