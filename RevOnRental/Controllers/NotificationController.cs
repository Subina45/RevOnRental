using MediatR;
using Microsoft.AspNetCore.Mvc;
using RevOnRental.Application.Interfaces;
using RevOnRental.Application.Services.Notifications.Command;
using RevOnRental.Domain.Models;

namespace RevOnRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("createNotification")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommand command)
        {
            if (command == null)
                return BadRequest("Invalid notification data.");

            try
            {
                var notification = await _mediator.Send(command);
                return Ok(notification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
