using Microsoft.EntityFrameworkCore;
using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public class WorkTaskRepository : IWorkTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateWorkTaskAsync(WorkTask workTask)
        {
            _context.WorkTasks.Add(workTask);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWorkTaskByIdAsync(string workTaskId)
        {
            var workTask = await _context.WorkTasks.FindAsync(workTaskId);
            if (workTask != null)
            {
                _context.WorkTasks.Remove(workTask);
                await _context.SaveChangesAsync(); 
            }
        }

        public async Task<List<WorkTask>> GetAllWorkTasks(string userId)
        {
            try
            {
                var workTask = await _context.WorkTasks.Where(task => task.UserId == userId).ToListAsync();

                return workTask;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving work tasks.", ex);
            }
        }

        public async Task<WorkTask?> GetWorkTaskByIdAsync(string workTaskId)
        {
            return await _context.WorkTasks.FirstOrDefaultAsync(t => t.WorkTaskId == workTaskId);
        }

        public async Task<WorkTask> GetWorkTaskByIdAsync(string workTaskId, string userId)
        {
            try
            {
                var workTask = await _context.WorkTasks
                    .FirstOrDefaultAsync(task => task.WorkTaskId == workTaskId && task.UserId == userId);

                return workTask;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving the work task.", ex);
            }
        }

        public async Task UpdateWorkTaskAsync(WorkTask workTask)
        {
            _context.WorkTasks.Update(workTask);
            await _context.SaveChangesAsync();
        }
    }
}
