using Microsoft.EntityFrameworkCore;
using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken && !r.IsRevoked);
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(string tokenId, string reason)
        {
            var token = await _context.RefreshTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedReason = reason;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllRefreshTokensForUserAsync(string userId, string reason)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedReason = reason;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsRefreshTokenValidAsync(string refreshToken, string ipAddress, string userAgent)
        {
            var token = await GetRefreshTokenAsync(refreshToken);
            if (token == null) return false;

            // Check if token is expired
            if (token.ExpiryDate < DateTime.UtcNow) return false;

            // Check if token was revoked
            if (token.IsRevoked) return false;

            // Check if token is being used from a different device/IP
            if (token.IPAddress != ipAddress || token.UserAgent != userAgent)
            {
                // Revoke the token as it might be compromised
                await RevokeRefreshTokenAsync(token.TokenID, "Suspicious activity detected");
                return false;
            }

            return true;
        }
    }
}
