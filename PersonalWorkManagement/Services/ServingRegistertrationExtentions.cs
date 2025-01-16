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
            services.AddHttpContextAccessor();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<UserService>();
            services.AddScoped<WorkTaskServices>();
            services.AddScoped<ApointmentService>();
            services.AddScoped<NoteService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWorkTaskRepository, WorkTaskRepository>();
            services.AddScoped<IApointmentRepository, ApointmentRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
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
