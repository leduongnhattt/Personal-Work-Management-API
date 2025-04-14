using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public interface ISocialLinkRepository
    {
        Task<SocialLink?> GetSocialLinkByIdAsync(string socialLinkId);
        Task<List<SocialLink>> GetSocialLinksByUserIdAsync(string userId);
        Task<SocialLink?> GetSocialLinkByPlatformAsync(string userId, string platform);
        Task CreateSocialLinkAsync(SocialLink socialLink);
        Task UpdateSocialLinkAsync(SocialLink socialLink);
        Task DeleteSocialLinkAsync(string socialLinkId);
    }
} 