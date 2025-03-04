using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public interface IRefreshTokenRepository
    {
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    }
}
