using MediatR;
using Microsoft.EntityFrameworkCore;
using RevOnRental.Application.Interfaces;
using RevOnRental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Services.Businesses.Queries
{
    public class GetBusinessDetailsQuery : IRequest<BusinessDto>
    {
        public int BusinessId { get; set; }

    }

    public class GetBusinessDetailsHandler : IRequestHandler<GetBusinessDetailsQuery, BusinessDto>
    {
        private readonly IAppDbContext _context;

        public GetBusinessDetailsHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<BusinessDto> Handle(GetBusinessDetailsQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Businesses
                .Include(b => b.Vehicles)
                .Include(b => b.Payments)
                .Include(b => b.Reviews)
                .Include(b => b.UserBusiness)
                .ThenInclude(b => b.User)
                .Select(x=>new BusinessDto
                {
                     Id = x.Id,
                     BusinessName = x.BusinessName,
                     BusinessType = x.BusinessType,
                     BusinessRegistrationNumber = x.BusinessRegistrationNumber,
                     PhoneNumber=x.UserBusiness.User.ContactNumber
                })
                .FirstOrDefaultAsync(b => b.Id == request.BusinessId, cancellationToken);
            if (data == null)
                throw new NullReferenceException();

            return data;
        }
    }
}
