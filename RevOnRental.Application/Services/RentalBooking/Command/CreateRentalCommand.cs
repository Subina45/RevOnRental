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

namespace RevOnRental.Application.Services.RentalBooking.Command
{
    public class CreateRentalCommand : IRequest<int> // Return the rental ID upon success
    {
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class CreateRentalHandler : IRequestHandler<CreateRentalCommand, int>
    {
        private readonly IAppDbContext _context;

        public CreateRentalHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
        {

            // Validate the vehicle availability
            var vehicle = await _context.Vehicles
                .Include(v => v.Rentals)
                .FirstOrDefaultAsync(v => v.Id == request.VehicleId, cancellationToken);

            if (vehicle == null)
            {
                throw new ArgumentException("Vehicle not found.");
            }

            if (!vehicle.AvailabilityStatus)
            {
                throw new InvalidOperationException("Vehicle is not available for rent.");
            }

            //// Check if the vehicle is already rented during the specified time period
            //bool isVehicleBooked = vehicle.Rentals.Any(r =>
            //    r.StartDate < request.EndDateTime && r.EndDate > request.StartDateTime);

            //if (isVehicleBooked)
            //{
            //    throw new InvalidOperationException("Vehicle is already booked for the selected time period.");
            //}

            // Calculate rental duration and price
            TimeSpan rentalDuration = request.EndDateTime - request.StartDateTime;
            decimal totalPrice;

            // Example pricing logic based on duration
            if (rentalDuration.TotalHours < 6)
            {
                totalPrice = (decimal)vehicle.RentalCharges.HourlyRate * (decimal)rentalDuration.TotalHours;
            }
            else if (rentalDuration.TotalHours == 6)
            {
                totalPrice = (decimal)vehicle.RentalCharges.HalfDayRate;
            }
            else
            {
                totalPrice =(decimal)vehicle.RentalCharges.FullDayRate;
            }

            // Create the rental record
            var rental = new Rental
            {
                UserID = request.UserId,
                VehicleId = request.VehicleId,
                StartDate = request.StartDateTime,
                EndDate = request.EndDateTime,
                TotalPrice = totalPrice,
                RentalStatus = RentalStatusType.Pending, // Default status can be changed to "Active" after confirmation
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            // Save the rental record to the database
            await _context.Rentals.AddAsync(rental, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Optionally update the vehicle's availability status
            vehicle.NumberOfAvailableVehicle = vehicle.NumberOfAvailableVehicle - 1;
            vehicle.AvailabilityStatus = vehicle.NumberOfAvailableVehicle==0 ? false : true;
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync(cancellationToken);

            // Return the rental ID
            return rental.Id;
        }
    }
}
