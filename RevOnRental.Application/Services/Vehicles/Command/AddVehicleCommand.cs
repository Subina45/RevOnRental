﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using RevOnRental.Application.Interfaces;
using RevOnRental.Application.Services.Businesses.Queries;
using RevOnRental.Domain.Enums;
using RevOnRental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Services.Vehicles.Command
{
    public class AddVehicleCommand : IRequest<bool>
    {
        public int BusinessId { get; set; }
        public VehicleType VehicleType { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public int NumberOfVehicle { get; set; }

        public bool AvailabilityStatus { get; set; }
        public float HourlyRate { get; set; }
        public float HalfDayRate { get; set; }
        public float FullDayRate { get; set; }
    }

    public class AddVehicleCommandHandler : IRequestHandler<AddVehicleCommand, bool>
    {
        private readonly IAppDbContext _context;

        public AddVehicleCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddVehicleCommand request, CancellationToken cancellationToken)
        {
            var vehicles = new Vehicle
            {
                BusinessID = request.BusinessId,
                VehicleType = request.VehicleType,
                Model = request.Model,
                Brand = request.Brand,
                NumberOfVehicle = request.NumberOfVehicle,
                AvailabilityStatus = request.AvailabilityStatus,
                RentalCharges = new RentalCharge
                {
                    HourlyRate = request.HourlyRate,
                    FullDayRate = request.FullDayRate,
                    HalfDayRate = request.HalfDayRate,
                }
            };
            await _context.Vehicles.AddAsync(vehicles);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
