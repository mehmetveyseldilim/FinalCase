using System.Diagnostics;

namespace Banking.API.Middleware
{
    public class MeasureResponseTimeAsyncCalculatorMiddleware
    {
        private const string X_RESPONSE_TIME_MS = "X-Response-Time-ms";
        private readonly ILogger<MeasureResponseTimeAsyncCalculatorMiddleware> _logger;

        private readonly RequestDelegate _next;

        public MeasureResponseTimeAsyncCalculatorMiddleware(RequestDelegate next, 
        ILogger<MeasureResponseTimeAsyncCalculatorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task InvokeAsync(HttpContext httpContext)
        {
            var watch = new Stopwatch();
            _logger.LogDebug("Startwatch has been started");
            watch.Start();

            httpContext.Response.OnStarting(() =>
            {
                watch.Stop();
                var responseTimeForCompleteRequest = watch.ElapsedMilliseconds;
                httpContext.Response.Headers[X_RESPONSE_TIME_MS] = responseTimeForCompleteRequest.ToString();
                return Task.CompletedTask;
            });
            

            return _next(httpContext);
        }
    }
}