using Microsoft.EntityFrameworkCore;
using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null!");
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserById(Guid userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            return await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<User?> GetUserByUserName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty!");
            }
            return await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task UpdateUserAsync(User updatedUser)
        {
            if (updatedUser == null)
            {
                throw new ArgumentNullException(nameof(updatedUser), "User cannot be null");
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == updatedUser.UserId);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            existingUser.UserName = updatedUser.UserName ?? existingUser.UserName;
            existingUser.Email = updatedUser.Email ?? existingUser.Email;
            existingUser.SDT = updatedUser.SDT ?? existingUser.SDT;
            existingUser.ImageUrl = updatedUser.ImageUrl ?? existingUser.ImageUrl;


            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

        }
    }
}
