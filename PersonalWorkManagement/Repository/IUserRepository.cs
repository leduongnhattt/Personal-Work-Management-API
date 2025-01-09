using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUserName(string username);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}
