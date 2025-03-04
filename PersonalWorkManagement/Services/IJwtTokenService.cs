using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Services
{
    public interface IJwtTokenService
    {
        public string GenerateToken(User user);
        public string GenerateRefreshToken();
    }
}
