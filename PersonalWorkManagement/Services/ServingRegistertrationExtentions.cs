using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PersonalWorkManagement.Models;
using PersonalWorkManagement.Repository;

namespace PersonalWorkManagement.Services
{
    public static class ServingRegistertrationExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ConnectDefault"));
            });

            return services;
        }
    }
}
