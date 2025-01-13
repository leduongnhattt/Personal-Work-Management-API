using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public interface IWorkTaskRepository
    {
        Task CreateWorkTaskAsync(WorkTask workTask);
        Task<WorkTask?> GetWorkTaskByIdAsync(Guid workTaskId);
        Task UpdateWorkTaskAsync(WorkTask workTask);
        Task DeleteWorkTaskByIdAsync(Guid workTaskId);
        Task<List<WorkTask>> GetAllWorkTasks(Guid userId);
        Task<WorkTask> GetWorkTaskByIdAsync(Guid workTaskId, Guid userId);
    }
}
