using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public interface IApointmentRepository
    {
        Task CreateApointmentAsync(Apointment apointment);
        Task DeleteApointmentAsync(string apointmentId);
        Task UpdateApointmentAsync(Apointment apointment);
        Task<List<Apointment>> GetAllApointmentAsync(string userId);
        Task<Apointment> GetApointmentByIdAsync(string apointmentId, string userId);
    }
}
