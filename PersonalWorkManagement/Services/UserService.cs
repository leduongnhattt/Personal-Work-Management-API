﻿
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

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IJwtTokenService jwtTokenService, IHttpContextAccessor contextAccessor)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _contextAccessor = contextAccessor;
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

        public async Task<ServiceResponse<string>> LoginUserAsync(UserDTO userDTO)
        {
            var response = new ServiceResponse<string>();
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
            response.Success = true;
            response.Message = "Login successful.";
            response.Data = token;
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
            var currentUserId = Guid.Parse(userIdClaim.Value);
            User user = await _userRepository.GetUserById(currentUserId);
            var data = new ProfileUserDTO
            {
                UserId = currentUserId,
                UserName = user.UserName,
                Email = user.Email,
                SDT = user.SDT
            };
            response.Success = true;
            response.Data = data;
            return response;
        }
    }
}
