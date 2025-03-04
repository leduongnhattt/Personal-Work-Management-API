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

        public async Task<ServiceResponse<object>> GetScheduleAsync(string userId)
        {
            var response = new ServiceResponse<object>();
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (currentUser == null)
            {
                response.Success = false;
                response.Message = "User is not authenticated or user ID is missing.";
                return response;
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var theNextTomorrow = today.AddDays(2);

            var tasks = await _context.WorkTasks
                .OrderBy(t => t.StartDateTask)
                .Where(t => t.UserId == userId &&
                            (t.StartDateTask.Date == today ||
                             t.StartDateTask.Date == tomorrow ||
                             t.StartDateTask.Date == theNextTomorrow))
                .Select(x => new
                {
                    x.WorkTaskId,
                    x.Title,
                    x.Description,
                    x.StartDateTask,
                    x.EndDateTask,
                    x.Status,
                    x.ReminderTime
                })
                .ToListAsync();

            var apointments = await _context.Apointsments.OrderBy(t => t.StartDateApoint)
                .Where(a => a.UserId == userId &&
                            (a.StartDateApoint.Date == today ||
                             a.StartDateApoint.Date == tomorrow ||
                             a.StartDateApoint.Date == theNextTomorrow))
                .Select(x => new
                {
                    x.ApointmentId,
                    x.Title,
                    x.Description,
                    x.Location,
                    x.StartDateApoint,
                    x.EndDateApoint,
                    x.ReminderTime
                })
                .ToListAsync();

            response.Success = true;
            response.Message = "Retrieved Schedule successfully!";
            response.Data = new
            {
                Tasks = tasks,
                Apointments = apointments
            };

            return response;
        }

    }
}
