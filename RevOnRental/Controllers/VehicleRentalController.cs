using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevOnRental.Application.Dtos;
using RevOnRental.Application.Dtos.BusinessDashboard;
using RevOnRental.Application.Dtos.BusinessDetails;
using RevOnRental.Application.Services.Businesses.Queries;
using RevOnRental.Application.Services.RentalBooking.Command;
using RevOnRental.Application.Services.Vehicles.Queries;
using RevOnRental.Domain.Enums;
using RevOnRental.Domain.Models;
using System.Numerics;

namespace RevOnRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleRentalController : BaseController
    {
        [HttpPost("search")]
        public async Task<ActionResult<List<VehicleAvailabilityDto>>> SearchAvailableVehicles([FromBody] SearchAvailableVehiclesQuery requestDto)
        {

            var availableVehicles = await Mediator.Send(requestDto);
            return Ok(availableVehicles);
        }

        [HttpGet("business-details/{businessId}")]
        public async Task<ActionResult<BusinessDetailsDto>> GetBusinessDetails(int businessId)
        {
            var query = new GetBusinessDetailsQuery{ BusinessId=businessId};
            var businessDetails = await Mediator.Send(query);
            return Ok(businessDetails);
        }

        [HttpPost("create-rent-vehicle")]
        public async Task<ActionResult<int>> RentVehicle([FromBody] CreateRentalCommand rentalDto)
        {
            var rentalId = await Mediator.Send(rentalDto);
            return Ok(rentalId);
        }

        [HttpPost("accept-rental")]
        public async Task<ActionResult<bool>> AcceptRental([FromBody] AcceptRentalCommand acceptRentalCommand)
        {
            var result = await Mediator.Send(acceptRentalCommand);
            return Ok(result);
        }

        [HttpPost("reject-rental")]
        public async Task<ActionResult<bool>> RejectRental([FromBody] RejectRentalCommand rejectRentalCommand)
        {
            var result = await Mediator.Send(rejectRentalCommand);
            return Ok(result);
        }

        [HttpGet("business-dashboard/{businessId}")]
        public async Task<ActionResult<BusinessDashboardDto>> GetBusinessDashboard(int businessId)
        {
            var query = new GetBusinessDashboardQuery { BusinessId= businessId };
            var dashboardData = await Mediator.Send(query);
            return Ok(dashboardData);
        }



        [HttpGet("vehicle-type-details")]
        public async Task<ActionResult<List<VehicleTypeDetailsDto>>> GetVehicleTypeDetails(GetVehicleTypeDetailsQuery requestDto)
        {
            var vehicleTypeDetails = await Mediator.Send(requestDto);
            return Ok(vehicleTypeDetails);
        }
    }
}
