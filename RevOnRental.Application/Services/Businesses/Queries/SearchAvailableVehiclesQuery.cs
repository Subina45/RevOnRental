using MediatR;
using Microsoft.EntityFrameworkCore;
using RevOnRental.Application.Dtos;
using RevOnRental.Application.Interfaces;
using RevOnRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Services.Businesses.Queries
{
    public class SearchAvailableVehiclesQuery : IRequest<List<VehicleAvailabilityDto>>
    {
        public VehicleType VehicleType { get; set; }
        public TimeEnum RentalTime { get; set; }

        public SearchAvailableVehiclesQuery(VehicleType vehicleType, TimeEnum rentalTime)
        {
            VehicleType = vehicleType;
            RentalTime = rentalTime;
        }
    }

    public class SearchAvailableVehiclesHandler : IRequestHandler<SearchAvailableVehiclesQuery, List<VehicleAvailabilityDto>>
    {
        private readonly IAppDbContext _context;

        public SearchAvailableVehiclesHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VehicleAvailabilityDto>> Handle(SearchAvailableVehiclesQuery request, CancellationToken cancellationToken)
        {
            var availableVehicles = await _context.Vehicles
                .Include(v => v.Business)
                .Where(v => v.VehicleType == request.VehicleType && v.AvailabilityStatus)
                .Select(v => new VehicleAvailabilityDto
                {
                    BusinessName = v.Business.BusinessName,
                   // Price =request.RentalTime==TimeEnum.Hour? v.RentalCharges.HourlyRate : 
                   // request.RentalTime == TimeEnum.HalfDay ? v.RentalCharges.HalfDayRate : v.RentalCharges.FullDayRate

                })
                .ToListAsync(cancellationToken);

            return availableVehicles;
        }
    }
}
