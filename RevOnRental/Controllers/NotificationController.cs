using MediatR;
using Microsoft.AspNetCore.Mvc;
using RevOnRental.Application.Interfaces;
using RevOnRental.Application.Services.Notifications.Command;
using RevOnRental.Application.Services.Notifications.Queries;
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

        [HttpGet("notifications")]
        public async Task<IActionResult> GetAllNotifications([FromQuery] int? userId, [FromQuery] int? businessId)
        {
            if (userId == null && businessId == null)
                return BadRequest("Either userId or businessId must be provided.");

            var query = new GetAllNotificationsQuery { UserId = userId, BusinessId = businessId };
            var notifications = await _mediator.Send(query);

            return Ok(notifications);
        }

        // Get Unread Notification Count
        [HttpGet("notifications/unread-count")]
        public async Task<IActionResult> GetNotificationCount([FromQuery] int? userId, [FromQuery] int? businessId)
        {
            if (userId == null && businessId == null)
                return BadRequest("Either userId or businessId must be provided.");

            var query = new GetNotificationCountQuery { UserId = userId, BusinessId = businessId };
            var count = await _mediator.Send(query);

            return Ok(new { UnreadCount = count });
        }

        [HttpPost("notifications/mark-as-read")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadCommand command)
        {
            if (command == null || (command.UserId == null && command.BusinessId == null))
                return BadRequest("Invalid request. Either UserId or BusinessId must be provided.");

            var result = await _mediator.Send(command);

            if (result)
                return Ok(result);
            else
                return BadRequest("Failed to mark notification as read.");
        }
    }
}
