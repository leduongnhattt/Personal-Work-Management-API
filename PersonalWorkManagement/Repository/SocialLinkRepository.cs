using Microsoft.EntityFrameworkCore;
using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public class SocialLinkRepository : ISocialLinkRepository
    {
        private readonly ApplicationDbContext _context;

        public SocialLinkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SocialLink?> GetSocialLinkByIdAsync(string socialLinkId)
        {
            return await _context.SocialLinks.FindAsync(socialLinkId);
        }

        public async Task<List<SocialLink>> GetSocialLinksByUserIdAsync(string userId)
        {
            return await _context.SocialLinks
                .Where(sl => sl.UserId == userId)
                .ToListAsync();
        }

        public async Task<SocialLink?> GetSocialLinkByPlatformAsync(string userId, string platform)
        {
            return await _context.SocialLinks
                .FirstOrDefaultAsync(sl => sl.UserId == userId && sl.Platform == platform);
        }

        public async Task CreateSocialLinkAsync(SocialLink socialLink)
        {
            _context.SocialLinks.Add(socialLink);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSocialLinkAsync(SocialLink socialLink)
        {
            _context.SocialLinks.Update(socialLink);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSocialLinkAsync(string socialLinkId)
        {
            var socialLink = await GetSocialLinkByIdAsync(socialLinkId);
            if (socialLink != null)
            {
                _context.SocialLinks.Remove(socialLink);
                await _context.SaveChangesAsync();
            }
        }
    }
} 