using Microsoft.AspNetCore.Mvc;
using RevOnRental.Application.Dtos;
using RevOnRental.Application.Services.Businesses.Queries;
using RevOnRental.Domain.Enums;
using RevOnRental.Domain.Models;
using System.Numerics;

namespace RevOnRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleRentalController : BaseController
    {
       

        [HttpGet("search")]
        public async Task<ActionResult<List<VehicleAvailabilityDto>>> SearchAvailableVehicles(VehicleType vehicleType, TimeEnum rentalTime)
        {
            var query = new SearchAvailableVehiclesQuery(vehicleType, rentalTime);
            var availableVehicles = await Mediator.Send(query);
            return Ok(availableVehicles);
        }

        [HttpGet("business/{businessId}")]
        public async Task<ActionResult<BusinessDto>> GetBusinessDetails(int businessId)
        {
            var result = await Mediator.Send(new GetBusinessDetailsQuery { BusinessId = businessId });

            //var query = new GetBusinessDetailsQuery{ BusinessId=businessId};
            //var businessDetails = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
