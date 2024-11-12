using Microsoft.AspNetCore.Mvc;
using RevOnRental.Application.Services;
using RevOnRental.Application.Dtos;
using RevOnRental.Application.Interfaces;
using RevOnRental.Domain.Models;

namespace RevOnRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

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

        

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            if (result.Succeeded)
            {
                return Ok("Login successful");
            }
            return Unauthorized("Invalid credentials");
        }
    }

}
