using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RevOnRental.Application.Dtos;
using RevOnRental.Application.Interfaces;
using RevOnRental.Application.Services.Payments.Command;
using RevOnRental.Application.Services.Users.Queries;
using System.Text;

namespace RevOnRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : BaseController
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequestDto paymentRequest)
        {
            var user = await _mediator.Send( new GetUserDetailsQuery { UserId = paymentRequest.UserId });
            if (user == null)
            {
                return BadRequest("User not found.");
            }
            var url = "https://a.khalti.com/api/v2/epayment/initiate/";
            var amountInPaisa = (decimal.Parse(paymentRequest.Amount)*100).ToString();

            var createPaymentCommand = new CreatePaymentCommand
            {
                UserId = paymentRequest.UserId,
                VehicleId = paymentRequest.VehicleId,
                BusinessId = paymentRequest.BusinessId,
                RentalId=int.Parse(paymentRequest.PurchaseRentalId),
                TotalPrice = decimal.Parse(paymentRequest.Amount),
                PaymentDate = DateTime.Now
            };
            var paymentId = await _mediator.Send(createPaymentCommand);

            var payload = new
            {
                return_url = "http://localhost:4200/usernotification",
                website_url = "https://localhost:7275",
                amount = amountInPaisa,
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
        public async Task<IActionResult> CompletePayment([FromBody] CompletePaymentCommand completePaymentDto)
        {
            
            var result = await _mediator.Send(completePaymentDto);

            if (!result)
            {
                return BadRequest("Failed to complete payment.");
            }

            return Ok("Payment completed successfully.");
        }
    }
}
