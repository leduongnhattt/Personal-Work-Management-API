using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PersonalWorkManagement.Models;
using System.Security.Claims;

namespace PersonalWorkManagement.Services
{
    public class ScheduleService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public ScheduleService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<ServiceResponse<object>> GetScheduleAsync(Guid userId)
        {
            var response = new ServiceResponse<object>();
            var currentUser = await _context.Users.FindAsync(userId);
            if (currentUser == null)
            {
                response.Success = false;
                response.Message = "User is not authenticated or user ID is missing.";
                return response;
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var tasks = await _context.WorkTasks
                .Where(t => t.UserId == userId &&
                            t.StartDateTask >= today ||
                            t.StartDateTask < tomorrow.AddDays(1)) 
                .ToListAsync();

            var apointments = await _context.Apointsments
                .Where(a => a.UserId == userId &&
                            a.StartDateApoint >= today ||
                            a.StartDateApoint < tomorrow.AddDays(1))
                .ToListAsync();


            response.Success = true;
            response.Message = "Retrived Schedule successfully!";
            response.Data = new
            {
                Tasks = tasks.Select(x => new
                {
                    x.WorkTaskId,
                    x.Title,
                    x.Description,
                    x.StartDateTask,
                    x.EndDateTask,
                    x.Status,
                    x.ReminderTime
                }).ToArray(),
                Apointments = apointments.Select(x => new
                {
                    x.ApointmentId,
                    x.Title,
                    x.Description,
                    x.Location,
                    x.StartDateApoint,
                    x.EndDateApoint,
                    x.ReminderTime
                }).ToArray()
            };

            return response;
        }
    }
}
