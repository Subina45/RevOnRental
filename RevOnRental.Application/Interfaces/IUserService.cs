using RevOnRental.Application.Dtos;
using RevOnRental.Application.Services.Users.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserDetailsQuery(int userId);
    }
}
