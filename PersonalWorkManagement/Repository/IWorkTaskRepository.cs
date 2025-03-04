using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public interface IWorkTaskRepository
    {
        Task CreateWorkTaskAsync(WorkTask workTask);
        Task<WorkTask?> GetWorkTaskByIdAsync(string workTaskId);
        Task UpdateWorkTaskAsync(WorkTask workTask);
        Task DeleteWorkTaskByIdAsync(string workTaskId);
        Task<List<WorkTask>> GetAllWorkTasks(string userId);
        Task<WorkTask> GetWorkTaskByIdAsync(string workTaskId, string userId);
    }
}
