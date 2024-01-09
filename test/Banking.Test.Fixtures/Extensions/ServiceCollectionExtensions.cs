
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Core;
using Quartz.Spi;

namespace Banking.Test.Fixtures.Extensions
{
     public static class ServiceCollectionExtensions
    {
        public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));

            if (descriptor != null) services.Remove(descriptor);
        }

        public static void RemoveQuartzScheduler(this IServiceCollection services)
        {
            var descriptorDbContext = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(ISchedulerFactory));
            services.Remove(descriptorDbContext);

            var descriptorStreamingClient = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IJobFactory));
            services.Remove(descriptorStreamingClient);

            var descriptorIDataClient = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IHostedService));
            services.Remove(descriptorIDataClient);

        }


        public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
        {
            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<T>();
            context.Database.Migrate();

            context.Database.EnsureCreated();    
        }
    }
}