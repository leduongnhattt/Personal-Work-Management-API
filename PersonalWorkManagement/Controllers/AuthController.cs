﻿using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Services;

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
            var response = await _userService.LoginUserAsync(userDTO);

            if (!response.Success)
            {
                return BadRequest(new { Status = "Failed", Message = response.Message });
            }

            return Ok(new { Token = response.Data, Message = response.Message, Status = "Success" });
        }
    }
}
