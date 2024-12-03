using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RevOnRental.Application.Dtos;
using RevOnRental.Application.Interfaces;
using RevOnRental.Application.Services.Payments.Command;
using System.Text;

namespace RevOnRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService; 

        public PaymentController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequestDto paymentRequest)
        {
            var user = await _userService.GetUserDetailsQuery(paymentRequest.UserId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            var url = "https://a.khalti.com/api/v2/epayment/initiate/";

            var createPaymentCommand = new CreatePaymentCommand
            {
                UserId = paymentRequest.UserId,
                VehicleId = paymentRequest.VehicleId,
                BusinessId = paymentRequest.BusinessId,
                TotalPrice = decimal.Parse(paymentRequest.Amount),
                PaymentDate = DateTime.UtcNow
            };
            var paymentId = await _mediator.Send(createPaymentCommand);

            var payload = new
            {
                return_url = "http://localhost:4200/usernotification",
                website_url = "https://localhost:7275",
                amount = paymentRequest.Amount,
                purchase_order_id = paymentRequest.PurchaseRentalId,
                purchase_order_name = paymentRequest.PurchaseRentalId,
                customer_info = new
                {
                    name = user.FullName,
                    email = user.Email,
                    phone = user.ContactNumber
                }
            };


            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "key 5be64226a89041efb66ffe13626b6037");

            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return Ok(responseContent);
        }
    
        [HttpPost("callback")]
        public async Task<IActionResult> CallBackApi(string test)
        {
            return Ok();
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompletePayment([FromBody] CompletePaymentDto completePaymentDto)
        {
            // Complete Payment command
            var completePaymentCommand = new CompletePaymentCommand
            {
                TransactionId = completePaymentDto.TransactionId,
                UpdatedDate = DateTime.Now
            };
            var result = await _mediator.Send(completePaymentCommand);

            if (!result)
            {
                return BadRequest("Failed to complete payment.");
            }

            return Ok("Payment completed successfully.");
        }
    }
}
