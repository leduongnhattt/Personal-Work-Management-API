using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public interface IRefreshTokenRepository
    {
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task RevokeRefreshTokenAsync(string tokenId, string reason);
        Task RevokeAllRefreshTokensForUserAsync(string userId, string reason);
        Task<bool> IsRefreshTokenValidAsync(string refreshToken, string ipAddress, string userAgent);
    }
}
