using Banking.API.ActionFilters;
using Banking.API.Extensions;
using Banking.API.Helper;


namespace Banking 
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Host.ConfigureSerilog(builder.Configuration);
            builder.Services.ConfigureValidations();
            builder.Services.AddScoped<FluentValidationFilter>();
            builder.Services.AddPostgresDbContext(builder.Configuration);
            builder.Services.ConfigureRepositories();
            builder.Services.ConfigureServices();
            builder.Services.AddAutoMapperService();

            builder.Services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
            });

            builder.Services.AddAuthentication();
            builder.Services.ConfigureIdentity();
            builder.Services.ConfigureJWT(builder.Configuration);
            builder.Services.AddJwtConfiguration(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseCustomMeasureResponseTimeAsyncMiddleware();
            app.UseCustomGlobalExceptionHandlerMiddleware();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            await app.Seed(builder.Environment);

            // app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
