using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Services;
using System.Security.Claims;

namespace PersonalWorkManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDTO registerUserDTO)
        {
            if (registerUserDTO == null)
            {
                return BadRequest(new { Status = "Error", Message = "Invalid user data." });
            }

            var response = await _userService.RegisterUserAsync(registerUserDTO);

            if (!response.Success)
            {
                return Conflict(new { Status = "Conflict", Message = response.Message });
            }

            return Ok(new { Status = "Success", Message = response.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            if (userDTO == null)
            {
                return BadRequest(new { Message = "Invalid user data." });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var response = await _userService.LoginUserAsync(userDTO, ipAddress, userAgent);

            if (!response.Success)
            {
                return BadRequest(new { Status = "Failed", Message = response.Message });
            }

            Response.Cookies.Append("refreshToken", response.Data.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                AccessToken = response.Data.AccessToken,
                Status = "Success"
            });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { Status = "Failed", Message = "No refresh token found." });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var response = await _userService.RefreshTokenAsync(refreshToken);
            if (!response.Success)
            {
                // If refresh token is invalid, remove it from cookies
                Response.Cookies.Delete("refreshToken");
                return Unauthorized(new { Status = "Failed", Message = response.Message });
            }

            Response.Cookies.Append("refreshToken", response.Data.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new
            {
                AccessToken = response.Data.AccessToken,
                Status = "Success"
            });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUser()
        {

            var response = await _userService.GetUserProfile();
            if (!response.Success)
            {
                return BadRequest(new { Status = "Failed", Message = response.Message });
            }

            return Ok(new { Data = response.Data, Message = response.Message, Status = "Success" });
        }
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateUserDTO updateUserDTO, IFormFile? imageFile)
        {

            var response = await _userService.UpdateUserAsync(updateUserDTO, imageFile);

            if (!response.Success)
            {
                return BadRequest(new { Status = "Failed", Message = response.Message });
            }

            return Ok(new { Status = "Success", Message = response.Message});
        }
        [Authorize]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordUserDTO updatePasswordUserDTO)
        {

            var response = await _userService.UpdatePasswordAsync(updatePasswordUserDTO);

            if (!response.Success)
            {
                return BadRequest(new { Status = "Failed", Message = response.Message });
            }

            return Ok(new { Status = "Success", Message = response.Message});
        }
    }
    
}
