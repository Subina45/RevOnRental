using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RevOnRental.Application.Dtos;
using RevOnRental.Application.Dtos.BusinessDashboard;
using RevOnRental.Application.Dtos.BusinessDetails;
using RevOnRental.Application.Services.Businesses.Queries;
using RevOnRental.Application.Services.RentalBooking.Command;
using RevOnRental.Application.Services.Vehicles.Queries;
using RevOnRental.Domain.Enums;
using RevOnRental.Domain.Models;
using RevOnRental.SignalR;
using RevOnRental.SignalR.Interfaces;
using System.Numerics;

namespace RevOnRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleRentalController : BaseController
    {
        private readonly IHubContext<MessageHub> _messageHub;
        private readonly IUserConnectionManager _userConnectionManager;

        public VehicleRentalController(IHubContext<MessageHub> messageHub, IUserConnectionManager userConnectionManager)
        {
                _messageHub = messageHub;
            _userConnectionManager = userConnectionManager;
        }
        [HttpPost("search")]
        public async Task<ActionResult<List<VehicleAvailabilityDto>>> SearchAvailableVehicles([FromBody] SearchAvailableVehiclesQuery requestDto)
        {

            var availableVehicles = await Mediator.Send(requestDto);
            return Ok(availableVehicles);
        }

        [HttpPost("business-details")]
        public async Task<ActionResult<BusinessDetailsDto>> GetBusinessDetails([FromBody]GetBusinessDetailsQuery requestDto)
        {
            var businessDetails = await Mediator.Send(requestDto);
            return Ok(businessDetails);
        }

        [HttpPost("create-rent-vehicle")]
        public async Task<ActionResult<int>> RentVehicle([FromBody] CreateRentalCommand rentalDto)
        {
            var result = await Mediator.Send(rentalDto);

            //signalR integration
            await BroadCastUpdateSignal(result);

            return Ok(result);
        }

        [HttpPost("accept-rental")]
        public async Task<ActionResult<bool>> AcceptRental([FromBody] AcceptRentalCommand acceptRentalCommand)
        {
            var result = await Mediator.Send(acceptRentalCommand);
            await BroadCastUpdateSignal(result);

            return Ok(result);
        }

        [HttpPost("reject-rental")]
        public async Task<ActionResult<bool>> RejectRental([FromBody] RejectRentalCommand rejectRentalCommand)
        {
            var result = await Mediator.Send(rejectRentalCommand);
            await BroadCastUpdateSignal(result);

            return Ok(result);
        }

        [HttpPost("confirm")]
        public async Task<ActionResult<bool>> ConfirmRental([FromBody] ConfirmRentalCommand command)
        {
            var result = await Mediator.Send(command);
            await BroadCastUpdateSignal(result);

            return Ok(result);
           
        }

        // Complete Rental
        [HttpPost("complete")]
        public async Task<ActionResult<bool>> CompleteRental([FromBody] CompleteRentalCommand command)
        {

            var result = await Mediator.Send(command);
            await BroadCastUpdateSignal(result);
            return Ok(result);
         
        }

        private async System.Threading.Tasks.Task BroadCastUpdateSignal(NotificationDto notificationDto)
        {
            try
            {
                var currentUserIdSignalR = _userConnectionManager.GetUserConnections(CurrentUserId);
               
                if (currentUserIdSignalR != null && currentUserIdSignalR.Count > 0)
                {
                    var activeSignalIds = currentUserIdSignalR.ToList();

                    await _messageHub.Clients.AllExcept(activeSignalIds).SendAsync("NotificationSend", notificationDto);
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"From signalR {ex.Message}");
            }
        }

        [HttpGet("business-dashboard/{businessId}")]
        public async Task<ActionResult<BusinessDashboardDto>> GetBusinessDashboard(int businessId)
        {
            var query = new GetBusinessDashboardQuery { BusinessId= businessId };
            var dashboardData = await Mediator.Send(query);
            return Ok(dashboardData);
        }



        [HttpPost("vehicle-type-details")]
        public async Task<ActionResult<List<VehicleTypeDetailsDto>>> GetVehicleTypeDetails([FromBody] GetVehicleTypeDetailsQuery requestDto)
        {
            var vehicleTypeDetails = await Mediator.Send(requestDto);
            return Ok(vehicleTypeDetails);
        }
    }
}
