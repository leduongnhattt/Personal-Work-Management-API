using Microsoft.EntityFrameworkCore;

namespace PersonalWorkManagement.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<WorkTask> WorkTasks { get; set; }
        public DbSet<Apointment> Apointsments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
