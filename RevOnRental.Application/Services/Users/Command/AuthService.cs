using Microsoft.AspNetCore.Identity;
using RevOnRental.Application.Dtos;
using RevOnRental.Application.Interfaces;
using RevOnRental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Services.Users.Command
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAppDbContext _appDbContext;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IAppDbContext appDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appDbContext = appDbContext;
        }

        // User Registration
        public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerUserDto.Email);

                if (existingUser != null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Email already exists" });
                }
                var user = new User
                {
                    UserName = registerUserDto.Email,
                    Email = registerUserDto.Email,
                    FirstName = registerUserDto.FirstName,
                    LastName = registerUserDto.LastName,
                    ContactNumber = registerUserDto.ContactNumber,
                    Address = registerUserDto.Address,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, registerUserDto.Password);
               
                if (result.Succeeded)
                {
                  await _userManager.AddToRoleAsync(user, "User");
                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        // Business Registration
        public async Task<IdentityResult> RegisterBusinessAsync(RegisterBusinessDto registerBusinessDto)
        {
            var user = new User
            {
                UserName = registerBusinessDto.Email,
                Email = registerBusinessDto.Email,
                FirstName = registerBusinessDto.FirstName,
                LastName = registerBusinessDto.LastName,
                ContactNumber = registerBusinessDto.ContactNumber,
                Address = registerBusinessDto.Address
            };

            var result = await _userManager.CreateAsync(user, registerBusinessDto.Password);
            if (result.Succeeded)
            {
                var getUser=await _userManager.FindByEmailAsync(registerBusinessDto.Email);
                await _userManager.AddToRoleAsync(user, "Business");

                var business = new Business
                {
                    BusinessName = registerBusinessDto.BusinessName,
                    BusinessRegistrationNumber = registerBusinessDto.BusinessRegistrationNumber,
                    BusinessType = registerBusinessDto.BusinessType

                };
                await _appDbContext.Businesses.AddAsync(business);
                await _appDbContext.SaveChangesAsync();

                var userBusiness = new UserBusiness
                {
                     BusinessId=business.Id,
                     UserId= getUser.Id
                };
                await _appDbContext.UserBusiness.AddAsync(userBusiness);

                await _appDbContext.SaveChangesAsync();

            }

            return result;
        }

        // Login Method
        public async Task<SignInResult> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user != null)
            {
                return await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
            }
            return SignInResult.Failed;
        }

        public async Task<bool> CreateRole(RoleDto roleDto)
        {
            var role = new Role
            {
                Name = roleDto.Name,
            };
            await _appDbContext.Roles.AddAsync(role);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<RoleDto>> GelAllRole()
        {
            
            var roles= _appDbContext.Roles.Select(x=>new RoleDto
            {
                 Id = x.Id,
                 Name=x.Name
            }).ToList();
            return roles;
        }
    }
}
