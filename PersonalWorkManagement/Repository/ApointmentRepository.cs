using Microsoft.EntityFrameworkCore;
using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public class ApointmentRepository : IApointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public ApointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateApointmentAsync(Apointment apointment)
        {
             _context.Apointsments.Add(apointment);
             await _context.SaveChangesAsync();
        }

        public async Task DeleteApointmentAsync(Guid apointmentId)
        {
            var apoinment = await _context.Apointsments.FindAsync(apointmentId);

            if (apoinment != null)
            {
                _context.Apointsments.Remove(apoinment);
                await _context.SaveChangesAsync();
            }
            throw new NotImplementedException("Not Found Apoinment");
        }

        public async Task<List<Apointment>> GetAllApointmentAsync(Guid userId)
        {
            return await _context.Apointsments.Where(a => a.UserId == userId).ToListAsync();
        }
        public async Task<Apointment?> GetApointmentByIdAsync(Guid apoinmentId)
        {
            return await _context.Apointsments.FirstOrDefaultAsync(a => a.ApointmentId == apoinmentId);
        }

        public async Task<Apointment> GetApointmentByIdAsync(Guid apoinmentId, Guid userId)
        {
            return await _context.Apointsments.FirstOrDefaultAsync(a => a.ApointmentId == apoinmentId && a.UserId == userId);
        }
        public async Task UpdateApointmentAsync(Apointment apointment)
        {
            _context.Apointsments.Update(apointment);
            await _context.SaveChangesAsync();
        }
    }
}
