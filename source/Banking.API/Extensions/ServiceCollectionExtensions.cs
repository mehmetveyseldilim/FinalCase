using System.Text;
using Banking.API.Validations;
using Banking.Domain.Contracts;
using Banking.Domain.Services;
using Banking.Persistance;
using Banking.Persistance.Entities;
using Banking.Shared;
using Banking.Shared.DTOs.Request;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Banking.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostgresDbContext(this IServiceCollection services, IConfiguration configuration) 
        {
            string connectionString = configuration.GetConnectionString("Postgres")!;

            Console.WriteLine($"Connection string is: {connectionString}");

            if(string.IsNullOrEmpty(connectionString)) 
            {
                Console.WriteLine("Connection string is null or empty");
                return services;
            }

            services.AddDbContext<BankingDbContext>(options => 
            {
                options.UseNpgsql(connectionString, b => 
                    b.MigrationsAssembly("Banking.API")
                    .MigrationsHistoryTable("__EFMigrationsHistory", schema: BankingDbContext.SCHEMA_NAME));

            });

            return services;
        }

        public static IServiceCollection AddAutoMapperService(this IServiceCollection services) 
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }

        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<CustomUser, CustomRole>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<BankingDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfiguration = new JwtConfiguration();

            configuration.Bind(jwtConfiguration.Section, jwtConfiguration);

            var secretKey = Environment.GetEnvironmentVariable("SecretKey");

            Console.WriteLine($"Secret Key: {secretKey}");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfiguration.ValidIssuer,
                    ValidAudience = jwtConfiguration.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                };
            });

            return services;
        }

        public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration) 
        {
            services.Configure<JwtConfiguration>
                (configuration.GetSection("JwtSettings")).AddOptions<JwtConfiguration>().ValidateDataAnnotations();

            return services;
        }

        public static IServiceCollection ConfigureValidations(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CreateUserDTO>, CreateUserDTOValidator>();
            services.AddScoped<IValidator<LoginUserDTO>, LoginUserDTOValidator>();


            return services;
        }

        public static IServiceCollection ConfigureRepositories(this IServiceCollection services) 
        {
            // services.AddScoped<ITypeRepository, TypeRepository>();

            return services;

        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services) 
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            return services;

        }
    }
}