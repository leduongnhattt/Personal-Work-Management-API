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

        public async Task DeleteApointmentAsync(string apointmentId)
        {
            var apoinment = await _context.Apointsments.FindAsync(apointmentId);

            if (apoinment != null)
            {
                _context.Apointsments.Remove(apoinment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Apointment>> GetAllApointmentAsync(string userId)
        {
            return await _context.Apointsments.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task<Apointment> GetApointmentByIdAsync(string apoinmentId, string userId)
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
