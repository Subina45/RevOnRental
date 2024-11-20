using Microsoft.AspNetCore.Mvc;
using RevOnRental.Application.Services;
using RevOnRental.Application.Dtos;
using RevOnRental.Application.Interfaces;
using RevOnRental.Domain.Models;
using RevOnRental.Application.Services.Users.Command;
using RevOnRental.Application.Dtos.Auth;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using RevOnRental.Application.Services.Businesses.Command;

namespace RevOnRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService )//, UpdateUser updateUser)
        {
            _authService = authService;
            //_updateUser = updateUser;
        }

        [AllowAnonymous]
        // Register User
        [HttpPost("register/user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterUserAsync(model);
            if (result.Succeeded)
            {
                return Ok("User registered successfully");
            }
            return BadRequest(result.Errors);
        }

        [AllowAnonymous]
        // Register Business
        [HttpPost("register/business")]
        public async Task<IActionResult> RegisterBusiness([FromBody] RegisterBusinessDto model)
        {
            var result = await _authService.RegisterBusinessAsync(model);
           
            if (result.Succeeded)
            {
                return Ok("Business registered successfully");
            }
            return BadRequest(result.Errors);
        }
        // Register User
        [HttpPost("add/role")]
        public async Task<IActionResult> AddRole([FromBody] RoleDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.CreateRole(model);
            
            return Ok(result);
        }

        // Register User
        [HttpGet("getAllRole")]
        public async Task<IActionResult> GetAllRole()
        {
            
            var result = await _authService.GelAllRole();

            return Ok(result);
        }


        [AllowAnonymous]
        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                var result = await _authService.LoginAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
        //updateUser
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var isUpdated = await _authService.UpdateUserAsync(userId, updateUserDto);
                if (isUpdated)
                {
                    return Ok("User information updated successfully.");
                }

                return BadRequest("Failed to update user information.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
