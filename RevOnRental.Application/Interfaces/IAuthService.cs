using Microsoft.AspNetCore.Identity;
using RevOnRental.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<IdentityResult> RegisterBusinessAsync(RegisterBusinessDto registerBusinessDto);
        Task<SignInResult> LoginAsync(LoginDto loginDto);
        Task<bool> CreateRole(RoleDto roleDto);
        Task<List<RoleDto>> GelAllRole();
    }
}
