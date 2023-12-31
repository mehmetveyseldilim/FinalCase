using Banking.API.Middleware;

namespace Banking.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomGlobalExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            return builder;
        }

        public static IApplicationBuilder UseCustomHttpLoggingMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<LoggingMiddleware>();

            return builder;
        }

        public static IApplicationBuilder UseCustomMeasureResponseTimeAsyncMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<MeasureResponseTimeAsyncCalculatorMiddleware>();

            return builder;
        }
    }
}