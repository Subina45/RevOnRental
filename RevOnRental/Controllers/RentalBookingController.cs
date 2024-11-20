using Microsoft.AspNetCore.Mvc;
using RevOnRental.Application.Interfaces;
using RevOnRental.Application.Services.RentalBooking.Queries;

namespace RevOnRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalBookingController : BaseController
    {
        private readonly IRentalBooking _rentalBooking;

        public RentalBookingController(IRentalBooking rentalBooking)
        {
            _rentalBooking = rentalBooking;
            
        }

        [HttpGet("business/{businessId}")]
        public async Task<IActionResult> GetRentalsByBusiness(int businessId)
        {
            var rentals = await _rentalBooking.GetRentalsByBusinessIdQuery(businessId);
            if (rentals == null || !rentals.Any())
            {
                return NotFound("No rentals found for the specified business.");
            }
            return Ok(rentals);
        }

        [HttpGet("{userId}/booking-history")]
        public async Task<IActionResult> GetBookingHistory(int userId)
        {
            try
            {
                var rentals = await _rentalBooking.GetUserBookingHistoryQuery(userId);
                if (rentals == null || !rentals.Any())
                {
                    return NotFound("No booking history found for the specified user.");
                }

                return Ok(rentals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
