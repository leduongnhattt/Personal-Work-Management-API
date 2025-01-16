using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public interface IApointmentRepository
    {
        Task CreateApointmentAsync(Apointment apointment);
        Task DeleteApointmentAsync(Guid apointmentId);
        Task UpdateApointmentAsync(Apointment apointment);
        Task<List<Apointment>> GetAllApointmentAsync(Guid userId);
        Task<Apointment> GetApointmentByIdAsync(Guid apointmentId, Guid userId);
    }
}
