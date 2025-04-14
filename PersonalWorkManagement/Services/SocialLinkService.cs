using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Models;
using PersonalWorkManagement.Repository;
using System.Security.Claims;

namespace PersonalWorkManagement.Services
{
    public class SocialLinkService
    {
        private readonly ISocialLinkRepository _socialLinkRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public SocialLinkService(ISocialLinkRepository socialLinkRepository, IHttpContextAccessor contextAccessor)
        {
            _socialLinkRepository = socialLinkRepository;
            _contextAccessor = contextAccessor;
        }

        private string GetCurrentUserId()
        {
            if (_contextAccessor.HttpContext == null)
            {
                throw new UnauthorizedAccessException("HTTP context not available!");
            }

            var userIdClaim = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)
                           ?? _contextAccessor.HttpContext.User.FindFirst("sub");

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User not authenticated!");
            }

            return userIdClaim.Value;
        }

        public async Task<ServiceResponse<List<SocialLinkDTO>>> GetUserSocialLinksAsync()
        {
            var response = new ServiceResponse<List<SocialLinkDTO>>();
            var userId = GetCurrentUserId();

            var socialLinks = await _socialLinkRepository.GetSocialLinksByUserIdAsync(userId);
            response.Data = socialLinks.Select(sl => new SocialLinkDTO
            {
                SocialLinkId = sl.SocialLinkId,
                Platform = sl.Platform,
                Url = sl.Url
            }).ToList();

            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<List<SocialLinkDTO>>> AddMultipleSocialLinksAsync(CreateMultipleSocialLinksDTO socialLinksDTO)
        {
            var response = new ServiceResponse<List<SocialLinkDTO>>();
            var userId = GetCurrentUserId();

            if (socialLinksDTO?.SocialLinks == null || !socialLinksDTO.SocialLinks.Any())
            {
                response.Success = false;
                response.Message = "No social links provided.";
                return response;
            }

            var validPlatforms = new[] { "Facebook", "Twitter", "Instagram", "LinkedIn" };
            var invalidPlatforms = socialLinksDTO.SocialLinks
                .Select(sl => sl.Platform)
                .Where(p => !validPlatforms.Contains(p))
                .ToList();

            if (invalidPlatforms.Any())
            {
                response.Success = false;
                response.Message = $"Invalid platforms: {string.Join(", ", invalidPlatforms)}. Valid platforms are: {string.Join(", ", validPlatforms)}";
                return response;
            }

            var existingLinks = await _socialLinkRepository.GetSocialLinksByUserIdAsync(userId);
            var existingPlatforms = existingLinks.Select(sl => sl.Platform).ToList();

            var newSocialLinks = new List<SocialLink>();
            foreach (var socialLinkDTO in socialLinksDTO.SocialLinks)
            {
                if (existingPlatforms.Contains(socialLinkDTO.Platform))
                {
                    continue; // Skip if platform already exists
                }

                var socialLink = new SocialLink
                {
                    SocialLinkId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Platform = socialLinkDTO.Platform,
                    Url = socialLinkDTO.Url,
                    CreatedAt = DateTime.UtcNow
                };

                newSocialLinks.Add(socialLink);
                existingPlatforms.Add(socialLinkDTO.Platform);
            }

            if (!newSocialLinks.Any())
            {
                response.Success = false;
                response.Message = "All provided platforms already exist for this user.";
                return response;
            }

            foreach (var socialLink in newSocialLinks)
            {
                await _socialLinkRepository.CreateSocialLinkAsync(socialLink);
            }

            response.Data = newSocialLinks.Select(sl => new SocialLinkDTO
            {
                SocialLinkId = sl.SocialLinkId,
                Platform = sl.Platform,
                Url = sl.Url
            }).ToList();

            response.Success = true;
            response.Message = "Social links added successfully.";
            return response;
        }

        public async Task<ServiceResponse<SocialLinkDTO>> UpdateSocialLinkAsync(string socialLinkId, UpdateSocialLinkDTO socialLinkDTO)
        {
            var response = new ServiceResponse<SocialLinkDTO>();
            var userId = GetCurrentUserId();

            var socialLink = await _socialLinkRepository.GetSocialLinkByIdAsync(socialLinkId);
            if (socialLink == null)
            {
                response.Success = false;
                response.Message = "Social link not found.";
                return response;
            }

            if (socialLink.UserId != userId)
            {
                response.Success = false;
                response.Message = "You don't have permission to update this social link.";
                return response;
            }

            socialLink.Url = socialLinkDTO.Url;
            await _socialLinkRepository.UpdateSocialLinkAsync(socialLink);

            response.Data = new SocialLinkDTO
            {
                SocialLinkId = socialLink.SocialLinkId,
                Platform = socialLink.Platform,
                Url = socialLink.Url
            };
            response.Success = true;
            response.Message = "Social link updated successfully.";
            return response;
        }

        public async Task<ServiceResponse<string>> DeleteSocialLinkAsync(string socialLinkId)
        {
            var response = new ServiceResponse<string>();
            var userId = GetCurrentUserId();

            var socialLink = await _socialLinkRepository.GetSocialLinkByIdAsync(socialLinkId);
            if (socialLink == null)
            {
                response.Success = false;
                response.Message = "Social link not found.";
                return response;
            }

            if (socialLink.UserId != userId)
            {
                response.Success = false;
                response.Message = "You don't have permission to delete this social link.";
                return response;
            }

            await _socialLinkRepository.DeleteSocialLinkAsync(socialLinkId);

            response.Success = true;
            response.Message = "Social link deleted successfully.";
            return response;
        }
    }
} 