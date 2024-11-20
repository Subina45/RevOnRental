using MediatR;
using Microsoft.EntityFrameworkCore;
using RevOnRental.Application.Dtos;
using RevOnRental.Application.Interfaces;
using RevOnRental.Domain.Enums;
using RevOnRental.Domain.Models;
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
        public string CurrentAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DestinationAddress { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        
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

            // Calculate rental duration (hourly, half-day, full-day)
            var rentalDuration = (request.EndDateTime - request.StartDateTime).TotalHours;
            var rentalType = rentalDuration < 6 ? "Hourly" :
                             rentalDuration == 6 ? "HalfDay" : "FullDay";

            // Fetch available vehicles of the specified type
            var vehicles = await _context.Vehicles
                .Include(v => v.Business)
                .ThenInclude(x => x.UserBusiness).ThenInclude(x => x.User)
                .Where(v => v.VehicleType == request.VehicleType && v.AvailabilityStatus)
                .Select(v => new VehicleAvailabilityDto
                {
                    BusinessName = v.Business.BusinessName,
                    Distance = DistanceCalculator.GetDistance(
                    request.Latitude, request.Longitude,
                    v.Business.UserBusiness.User.Latitude, v.Business.UserBusiness.User.Longitude),
                    Rating = (int)Math.Round(v.Business.Reviews.Any() ? v.Business.Reviews.Average(r => r.Rating) : 0)
                }).OrderBy(z => z.Distance)
                .ToListAsync(cancellationToken);



            return vehicles;
        }

       
    }
}
