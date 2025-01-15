using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Models;
using PersonalWorkManagement.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PersonalWorkManagement.Services
{
    public class ApointmentService
    {
        private readonly IApointmentRepository _repository;
        private readonly IHttpContextAccessor _contextAccessor;

        public ApointmentService(IApointmentRepository repository, IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _contextAccessor = contextAccessor;
        }

        public async Task<ServiceResponse<string>> AddApointmentAsync(UpdateApointmentDTO apointmentDTO)
        {
            var response = new ServiceResponse<string>();

            if (!IsValidDateRange(apointmentDTO.StartDateApoint, apointmentDTO.EndDateApoint))
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
            if (IsNull(apointmentDTO))
            {
                response.Success = false;
                response.Message = "You must to text all fields";
                return response;
            }
            var apointment = new Apointment
            {
                ApointmentId = Guid.NewGuid(),
                Title = apointmentDTO.Title,
                Description = apointmentDTO.Description,
                Location = apointmentDTO.Location,
                StartDateApoint = apointmentDTO.StartDateApoint,
                EndDateApoint = apointmentDTO.EndDateApoint,
                ReminderTime = apointmentDTO.ReminderTime,
                UserId = currentUserId.Value,
            };
            await _repository.CreateApointmentAsync(apointment);
            response.Success = true;
            response.Message = "Apointment created successfully!";
            return response;
        }
        public async Task<ServiceResponse<string>> UpdateApointmentAsync(Guid apointmentId, UpdateApointmentDTO updateApointmentDTO)
        {
            var response = new ServiceResponse<string>();

            if (!IsValidDateRange(updateApointmentDTO.StartDateApoint, updateApointmentDTO.EndDateApoint))
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
            var apointment = await _repository.GetApointmentByIdAsync(apointmentId, currentUserId.Value);

            if (apointment == null || apointment.UserId != currentUserId)
            {
                response.Success = false;
                response.Message = "Apointment not found or you're not authorized to update this task.";
                return response;
            }
            UpdateApointment(apointment, updateApointmentDTO);
            await _repository.UpdateApointmentAsync(apointment);

            response.Success = true;
            response.Message = "Apointment updated successfully!";
            return response;
        }
        public async Task<ServiceResponse<string>> DeleteApointmentAsync(Guid apointmentId)
        {
            var response = new ServiceResponse<string>();

            if (apointmentId == null)
            {
                response.Success = false;
                response.Message = "Apointment is not null";
                return response;
            }
            var currentUserId = await GetAuthenticatedUserIdAsync();
            var apointment = await _repository.GetApointmentByIdAsync(apointmentId, currentUserId.Value);
            if (apointment == null || apointment.UserId != currentUserId)
            {
                response.Success = false;
                response.Message = "Apointment not found or you're not authorized to update this task.";
                return response;
            }
            await _repository.DeleteApointmentAsync(apointmentId);
            response.Success = true;
            response.Message = "Apointment deleted successfully!";
            return response;
        }
        public async Task<ServiceResponse<List<ApointmentDTO>>> GetAllApointmentAsync()
        {
            var response = new ServiceResponse<List<ApointmentDTO>>();
            var currentUserId = await GetAuthenticatedUserIdAsync();
            if (currentUserId == null)
            {
                response.Success = false;
                response.Message = "User not authenticated!";
                return response;
            }
            var apointments = await _repository.GetAllApointmentAsync(currentUserId.Value);
            if (apointments == null || !apointments.Any())
            {
                response.Success = false;
                response.Message = "No apointments found for this user.";
                return response;
            }
            response.Data = apointments.Select(wt => new ApointmentDTO
            {
                ApointmentId = wt.ApointmentId,
                Title = wt.Title,
                Description = wt.Description,
                Location = wt.Location,
                StartDateApoint = wt.StartDateApoint,
                EndDateApoint = wt.EndDateApoint,
                ReminderTime = wt.ReminderTime
            }).ToList();
            response.Success = true;
            response.Message = "Apointments retrieved successfully!";
            return response;
        }
        public async Task<ServiceResponse<ApointmentDTO>> GetApointmentByIdAsync(Guid apointmentId)
        {
            var response = new ServiceResponse<ApointmentDTO>();
            var currentUserId = await GetAuthenticatedUserIdAsync();
            if (currentUserId == null)
            {
                response.Success = false;
                response.Message = "User not authenticated!";
                return response;
            }
            var apointment = await _repository.GetApointmentByIdAsync(apointmentId, currentUserId.Value);
            if (apointment == null || apointment.UserId != currentUserId)
            {
                response.Success = false;
                response.Message = "Apointment not found or you're not authorized to view this task.";
                return response;
            }
            response.Data = new ApointmentDTO
            {
                ApointmentId = apointmentId,
                Title = apointment.Title,
                Description = apointment.Description,
                StartDateApoint = apointment.StartDateApoint,
                EndDateApoint = apointment.EndDateApoint,
                Location = apointment.Location,
                ReminderTime = apointment.ReminderTime
            };

            response.Success = true;
            response.Message = "Apointment retrieved successfully!";
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
        private void UpdateApointment(Apointment apointment, UpdateApointmentDTO updateApointmentDTO)
        {
            apointment.Title = updateApointmentDTO.Title;
            apointment.Description = updateApointmentDTO.Description;
            apointment.StartDateApoint = updateApointmentDTO.StartDateApoint;
            apointment.EndDateApoint = updateApointmentDTO.EndDateApoint;
            apointment.Location = updateApointmentDTO.Location;
            apointment.ReminderTime = updateApointmentDTO.ReminderTime;
        }
        private bool IsNull(UpdateApointmentDTO apointmentDTO)
        {
            return String.IsNullOrWhiteSpace(apointmentDTO.Title) ||
                   apointmentDTO.ReminderTime == null || 
                   apointmentDTO.EndDateApoint == null || 
                   apointmentDTO.StartDateApoint == null ||
                   String.IsNullOrWhiteSpace(apointmentDTO.Location) ||
                   String.IsNullOrWhiteSpace(apointmentDTO.Description);
        }
    }
}
