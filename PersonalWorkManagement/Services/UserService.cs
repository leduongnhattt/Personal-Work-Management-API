
using Azure;
using Microsoft.AspNetCore.Identity;
using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Models;
using PersonalWorkManagement.Repository;

namespace PersonalWorkManagement.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }
        public async Task<ServiceResponse<string>> RegisterUserAsync(RegisterUserDTO registerUserDTO)
        {
            var response = new ServiceResponse<string>();
            var existingUser = await _userRepository.GetUserByUserName(registerUserDTO.Username);
            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "Username already exists.";

                return response;
            }
            var user = new User
            {
                UserName = registerUserDTO.Username,
                Email = registerUserDTO.Email
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
    }
}
