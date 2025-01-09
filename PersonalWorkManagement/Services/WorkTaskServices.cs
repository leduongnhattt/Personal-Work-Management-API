using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Models;
using PersonalWorkManagement.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PersonalWorkManagement.Services
{
    public class WorkTaskServices
    {
        private readonly IWorkTaskRepository _workTaskRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public WorkTaskServices(IWorkTaskRepository workTaskRepository, IHttpContextAccessor contextAccessor)
        {
            _workTaskRepository = workTaskRepository;
            _contextAccessor = contextAccessor;
        }
        public async Task<ServiceResponse<string>> AddWorkTaskAsync(WorkTaskDTO workTaskDTO)
        {
            var response = new ServiceResponse<string>();

            // Validate task dates
            if (!IsValidDateRange(workTaskDTO.StartDateTask, workTaskDTO.EndDateTask))
            {
                response.Success = false;
                response.Message = "Start date must be earlier than end date.";
                return response;
            }

            // Validate user authentication
            var currentUserId = await GetAuthenticatedUserIdAsync();
            if (currentUserId == null)
            {
                response.Success = false;
                response.Message = "User not authenticated!";
                return response;
            }

            // Validate status
            if (!Enum.TryParse<StatusTask>(workTaskDTO.Status, true, out var status))
            {
                response.Success = false;
                response.Message = "Invalid status value!";
                return response;
            }

            // Create and save the work task
            var workTask = CreateWorkTask(workTaskDTO, currentUserId.Value, status);
            await _workTaskRepository.CreateWorkTaskAsync(workTask);

            response.Success = true;
            response.Message = "Work task created successfully!";
            return response;
        }

        // Public method to update an existing work task
        public async Task<ServiceResponse<string>> UpdateWorkTaskAsync(Guid workTaskId, WorkTaskDTO workTaskDTO)
        {
            var response = new ServiceResponse<string>();

            if (!IsValidDateRange(workTaskDTO.StartDateTask, workTaskDTO.EndDateTask))
            {
                response.Success = false;
                response.Message = "Start date must be earlier than end date.";
                return response;
            }
            var currentUserId = await GetAuthenticatedUserIdAsync();
            if (currentUserId == null)
            {
                response.Success = false;
                response.Message = "User not authenticated!";
                return response;
            }

            if (!Enum.TryParse<StatusTask>(workTaskDTO.Status, true, out var status))
            {
                response.Success = false;
                response.Message = "Invalid status value!";
                return response;
            }

            // Retrieve the existing work task
            var workTask = await _workTaskRepository.GetWorkTaskByIdAsync(workTaskId);
            if (workTask == null || workTask.UserId != currentUserId)
            {
                response.Success = false;
                response.Message = "Work task not found or you're not authorized to update this task.";
                return response;
            }
            UpdateWorkTask(workTask, workTaskDTO, status);

            await _workTaskRepository.UpdateWorkTaskAsync(workTask);

            response.Success = true;
            response.Message = "Work task updated successfully!";
            return response;
        }

        public async Task<ServiceResponse<string>> DeleteWorkTaskAsync(Guid workTaskId)
        {
            var response = new ServiceResponse<string>();
            if (workTaskId == null)
            {
                response.Success = false;
                response.Message = "Work Task is not null";
                return response;
            }
            var currentUserId = await GetAuthenticatedUserIdAsync();

            var workTask = await _workTaskRepository.GetWorkTaskByIdAsync(workTaskId);
            if (workTask == null || workTask.UserId != currentUserId)
            {
                response.Success = false;
                response.Message = "Work task not found or you're not authorized to update this task.";
                return response;
            }
            await _workTaskRepository.DeleteWorkTaskByIdAsync(workTaskId);
            response.Success = true;
            response.Message = "Work task deleted successfully!";
            return response;

        }

        public async Task<ServiceResponse<List<WorkTaskDTO>>> GetAllWorkTaskAsync()
        {
            var response = new ServiceResponse<List<WorkTaskDTO>>();
            var currentUserId = await GetAuthenticatedUserIdAsync();
            if (currentUserId == null)
            {
                response.Success = false;
                response.Message = "User not authenticated!";
                return response;
            }
            var workTasks = await _workTaskRepository.GetAllWorkTasks(currentUserId.Value);
            if (workTasks == null || !workTasks.Any())
            {
                response.Success = false;
                response.Message = "No tasks found for this user.";
                return response;
            }
            response.Data = workTasks.Select(wt => new WorkTaskDTO
            {
                Title = wt.Title,
                Description = wt.Description,
                StartDateTask = wt.StartDateTask,
                EndDateTask = wt.EndDateTask,
                Status = wt.Status.ToString(),
                ReminderTime = wt.ReminderTime
            }).ToList();
            response.Success = true;
            response.Message = "Work tasks retrieved successfully!";
            return response;
        }
        public async Task<ServiceResponse<WorkTaskDTO>> GetWorkTaskByIdAsync(Guid workTaskId)
        {
            var response = new ServiceResponse<WorkTaskDTO>();
            var currentUserId = await GetAuthenticatedUserIdAsync();
            if (currentUserId == null)
            {
                response.Success = false;
                response.Message = "User not authenticated!";
                return response;
            }
            var workTask = await _workTaskRepository.GetWorkTaskByIdAsync(workTaskId);
            if (workTask == null || workTask.UserId != currentUserId)
            {
                response.Success = false;
                response.Message = "Work task not found or you're not authorized to view this task.";
                return response;
            }
            response.Data = new WorkTaskDTO
            {
                Title = workTask.Title,
                Description = workTask.Description,
                StartDateTask = workTask.StartDateTask,
                EndDateTask = workTask.EndDateTask,
                Status = workTask.Status.ToString(),
                ReminderTime = workTask.ReminderTime
            };

            response.Success = true;
            response.Message = "Work task retrieved successfully!";
            return response;
        }
        private bool IsValidDateRange(DateTime startDate, DateTime endDate)
        {
            return startDate <= endDate;
        }
        private async Task<Guid?> GetAuthenticatedUserIdAsync()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier) ??
                              _contextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null) return null;

            return Guid.TryParse(userIdClaim.Value, out var userId) ? userId : (Guid?)null;
        }
        private WorkTask CreateWorkTask(WorkTaskDTO workTaskDTO, Guid userId, StatusTask status)
        {
            return new WorkTask
            {
                WorkTaskId = Guid.NewGuid(),
                Title = workTaskDTO.Title,
                Description = workTaskDTO.Description,
                StartDateTask = workTaskDTO.StartDateTask,
                EndDateTask = workTaskDTO.EndDateTask,
                Status = status,
                ReminderTime = workTaskDTO.ReminderTime,
                UserId = userId,
            };
        }

        // Private helper method to update a WorkTask instance with new data
        private void UpdateWorkTask(WorkTask workTask, WorkTaskDTO workTaskDTO, StatusTask status)
        {
            workTask.Title = workTaskDTO.Title;
            workTask.Description = workTaskDTO.Description;
            workTask.StartDateTask = workTaskDTO.StartDateTask;
            workTask.EndDateTask = workTaskDTO.EndDateTask;
            workTask.Status = status;
            workTask.ReminderTime = workTaskDTO.ReminderTime;
        }
    }
}
