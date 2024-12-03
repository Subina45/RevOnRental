using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RevOnRental.Application.Dtos;
using System.Text;

namespace RevOnRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequestDto paymentRequest)
        {
            var url = "https://a.khalti.com/api/v2/epayment/initiate/";

            var payload = new
            {
                return_url = "http://localhost:4200/usernotification",
                website_url = "https://localhost:7275",
                amount = paymentRequest.Amount,
                purchase_order_id = paymentRequest.PurchaseRentalId,
                purchase_order_name = paymentRequest.PurchaseRentalId,
                customer_info = new
                {
                    name = paymentRequest.CustomerName,
                    email = paymentRequest.CustomerEmail,
                    phone = paymentRequest.CustomerPhone
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
}
}
