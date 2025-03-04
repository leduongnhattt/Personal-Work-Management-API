
using Azure;
using Microsoft.AspNetCore.Identity;
using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Models;
using PersonalWorkManagement.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PersonalWorkManagement.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly string _imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IJwtTokenService jwtTokenService, IHttpContextAccessor contextAccessor, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _contextAccessor = contextAccessor;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<ServiceResponse<string>> RegisterUserAsync(RegisterUserDTO registerUserDTO)
        {
            var response = new ServiceResponse<string>();
            var existingUser = await _userRepository.GetUserByUserName(registerUserDTO.Username);
            if (string.IsNullOrWhiteSpace(registerUserDTO.SDT) ||
                !System.Text.RegularExpressions.Regex.IsMatch(registerUserDTO.SDT, @"^0\d{9}$"))
            {
                response.Success = false;
                response.Message = "Invalid phone number. It must start with '0' and contain exactly 10 digits.";
                return response;
            }
            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "Username already exists.";

                return response;
            }
            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                UserName = registerUserDTO.Username,
                Email = registerUserDTO.Email,
                SDT = registerUserDTO.SDT,
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, registerUserDTO.Password);
            user.CreatedAt = DateTime.UtcNow;
            await _userRepository.CreateUserAsync(user);

            response.Success = true;
            response.Message = "User registered successfully.";
            response.Data = user.UserName; 
            return response;
        }

        public async Task<ServiceResponse<TokenResponseDTO>> LoginUserAsync(UserDTO userDTO)
        {
            var response = new ServiceResponse<TokenResponseDTO>();
            var user = await _userRepository.GetUserByUserName(userDTO.UserName);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            var passwordVerification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDTO.Password);
            if (passwordVerification != PasswordVerificationResult.Success)
            {
                response.Success = false;
                response.Message = "Incorrect password.";
                return response;
            }
            var token = _jwtTokenService.GenerateToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                TokenID = Guid.NewGuid().ToString(),
                UserId = user.UserId,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };
            await _refreshTokenRepository.SaveRefreshTokenAsync(refreshTokenEntity);
            response.Success = true;
            response.Message = "Login successful.";
            response.Data = new TokenResponseDTO
            {
                AccessToken = token,
                RefreshToken = refreshToken
            };
            return response;
        }
        public async Task<ServiceResponse<TokenResponseDTO>> RefreshTokenAsync(string refreshToken)
        {
            var response = new ServiceResponse<TokenResponseDTO>();
            var storedToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                response.Success = false;
                response.Message = "Invalid refresh token.";
                return response;
            }
            var user = await _userRepository.GetUserById(storedToken.UserId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            var newAccessToken = _jwtTokenService.GenerateToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            storedToken.Token = newRefreshToken;
            storedToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
            storedToken.CreatedAt = DateTime.UtcNow;
            storedToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateRefreshTokenAsync(storedToken);
            response.Data = new TokenResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
            response.Success = true;
            response.Message = "Token refreshed successfully.";
            return response;
        }
        public async Task<ServiceResponse<ProfileUserDTO>> GetUserProfile()
        {
            var response = new ServiceResponse<ProfileUserDTO>();
            var userIdClaim = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier) ??
                  _contextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                response.Success = false;
                response.Message = "User not authenticated!";
                return response;
            }
            var currentUserId = userIdClaim.Value;
            User user = await _userRepository.GetUserById(currentUserId);
            var data = new ProfileUserDTO
            {
                UserId = currentUserId,
                UserName = user.UserName,
                Email = user.Email,
                SDT = user.SDT,
                Image = user.ImageUrl
            };
            response.Success = true;
            response.Data = data;
            return response;
        }
        public async Task<ServiceResponse<UpdateUserDTO>> UpdateUserAsync(UpdateUserDTO updateUserProfileDTO, IFormFile? imageFile)
        {
            var response = new ServiceResponse<UpdateUserDTO>();
            var currentUserId = GetCurrentUserId();

            var existingUser = await _userRepository.GetUserById(currentUserId);

            if (existingUser == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            if (!string.IsNullOrEmpty(updateUserProfileDTO.UserName)) existingUser.UserName = updateUserProfileDTO.UserName;
            if (!string.IsNullOrEmpty(updateUserProfileDTO.Email)) existingUser.Email = updateUserProfileDTO.Email;
            if (!string.IsNullOrEmpty(updateUserProfileDTO.SDT)) existingUser.SDT = updateUserProfileDTO.SDT;

            if (imageFile != null && imageFile.Length > 0)
            {
                // Ensure the directory exists
                if (!Directory.Exists(_imageFolderPath))
                {
                    Directory.CreateDirectory(_imageFolderPath);
                }

                var filePath = Path.Combine(_imageFolderPath, Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName));

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                existingUser.ImageUrl = "/images/" + Path.GetFileName(filePath);
            }

            await _userRepository.UpdateUserAsync(existingUser);
            response.Data = new UpdateUserDTO
            {
                UserName = existingUser.UserName,
                Email = existingUser.Email,
                SDT = existingUser.SDT
            };

            response.Success = true;
            response.Message = "Profile updated successfully.";
            return response;
        }
        public async Task<ServiceResponse<string>> UpdatePasswordAsync(UpdatePasswordUserDTO updatePasswordUserDTO)
        {
            var response = new ServiceResponse<string>();

            var currentUserId = GetCurrentUserId();

            var existingUser = await _userRepository.GetUserById(currentUserId);

            if (existingUser == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            var verifyResult = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, updatePasswordUserDTO.OldPassword);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                response.Success = false;
                response.Message = "Old password is incorrect.";
                return response;
            }
            existingUser.PasswordHash = _passwordHasher.HashPassword(existingUser, updatePasswordUserDTO.NewPassword);
            await _userRepository.UpdateUserAsync(existingUser);

            response.Success = true;
            response.Message = "Password updated successfully.";
            return response;
        }

        private string GetCurrentUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
                               ?? _contextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User not authenticated!");
            }

            return userIdClaim.Value;
        }

    }
}
