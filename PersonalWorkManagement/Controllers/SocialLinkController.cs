using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Services;

namespace PersonalWorkManagement.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SocialLinkController : ControllerBase
    {
        private readonly SocialLinkService _socialLinkService;

        public SocialLinkController(SocialLinkService socialLinkService)
        {
            _socialLinkService = socialLinkService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserSocialLinks()
        {
            var response = await _socialLinkService.GetUserSocialLinksAsync();
            if (!response.Success)
            {
                return BadRequest(new { Status = "Failed", Message = response.Message });
            }
            return Ok(new { Data = response.Data, Status = "Success" });
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> AddMultipleSocialLinks([FromBody] CreateMultipleSocialLinksDTO socialLinksDTO)
        {
            if (socialLinksDTO == null || socialLinksDTO.SocialLinks == null || !socialLinksDTO.SocialLinks.Any())
            {
                return BadRequest(new { Status = "Error", Message = "Invalid social links data." });
            }

            var response = await _socialLinkService.AddMultipleSocialLinksAsync(socialLinksDTO);
            if (!response.Success)
            {
                return BadRequest(new { Status = "Failed", Message = response.Message });
            }
            return Ok(new { Data = response.Data, Status = "Success", Message = response.Message });
        }

        [HttpPut("{socialLinkId}")]
        public async Task<IActionResult> UpdateSocialLink(string socialLinkId, [FromBody] UpdateSocialLinkDTO socialLinkDTO)
        {
            if (socialLinkDTO == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid social link data." });
            }

            var response = await _socialLinkService.UpdateSocialLinkAsync(socialLinkId, socialLinkDTO);
            if (!response.Success)
            {
                return BadRequest(new { Status = "Failed", Message = response.Message });
            }
            return Ok(new { Data = response.Data, Status = "Success", Message = response.Message });
        }

        [HttpDelete("{socialLinkId}")]
        public async Task<IActionResult> DeleteSocialLink(string socialLinkId)
        {
            var response = await _socialLinkService.DeleteSocialLinkAsync(socialLinkId);
            if (!response.Success)
            {
                return BadRequest(new { Status = "Failed", Message = response.Message });
            }
            return Ok(new { Status = "Success", Message = response.Message });
        }
    }
} 