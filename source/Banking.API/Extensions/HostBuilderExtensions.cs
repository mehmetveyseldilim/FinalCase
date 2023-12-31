using Serilog;

namespace Banking.API.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureSerilog(this IHostBuilder builder, IConfiguration configuration)
        {
            builder.UseSerilog((context, configuration) => 
                                configuration.ReadFrom.Configuration(context.Configuration));

            return builder;
        }

    }
}