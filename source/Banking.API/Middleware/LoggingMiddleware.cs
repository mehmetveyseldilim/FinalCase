

namespace Banking.API.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public  Task Invoke(HttpContext httpContext)
        {
            var requestPath = httpContext.Request.Path;
            var action = httpContext.Request.Method;

            _logger.LogDebug("Request path: {@requestPath}", requestPath);
            _logger.LogDebug("Entering action {@action}", action);

            return _next(httpContext);
        }

    }
}