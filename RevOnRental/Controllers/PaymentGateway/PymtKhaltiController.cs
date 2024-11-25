using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RevOnRental.Application.Dtos;
using System.Text;
using RevOnRental.Domain.Models.PaymentGateway;
using RevOnRental.Application.Services.Vehicles.Command;
using static System.Net.WebRequestMethods;

namespace RevOnRental.Controllers.PaymentGateway
{
    [Route("api/Khalti/[action]")]
    [ApiController]
    public class PymtKhaltiController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> KhaltiIntegration([FromBody] KhaltiPayload khaltiPayload) //Modify Model as per requirement
        {

            {

                try
                {

                    var FinalAmnt = (Convert.ToDecimal(khaltiPayload.Amount) * 100); // This is for converting Rupees into Paisa
                    //var FinalAmnt = (Convert.ToDecimal(khaltiPayload.Amount) * 10); // For testing purpose

                    var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                    KhaltiDTO khalti = new KhaltiDTO();

                    //SAMPLE PAYLOAD FOR KHALTI PLEASE CREATE KhaltiMaster Table on database then insert related data

                    khalti.RURL = "https://localhost:44361/Khalti/KhaltiVerification";
                    khalti.URL = "https://localhost:44361/Khalti/KhaltiVerification";
                    khalti.SecretKey = "live_secret_key_68791341fdd94846a146f0457ff7b455";
                    khalti.KhaltiUrl = "https://a.khalti.com/api/v2/epayment/initiate/"; // For live : https://khalti.com/api/v2/epayment/initiate/ 
                    khalti.TradeName = "RevOnRental";
                    khalti.Email = "info@revonrental.com";
                    khalti.ContactNo = "061-5601237";

                    //SAMPLE PAYLOAD END

                    // Here You can create a service to get khalti related data along with related user info for khalti payload and append on Purchase  ; It may help you on verification process cause it returns data on query string
                    // var obj = JsonConvert.DeserializeObject<List<KhaltiDTO>>(result);

                    // khalti = obj.First();

                    var url = khalti.KhaltiUrl;
                    var purcahse_id = Guid.NewGuid().ToString();
                   
                    var Purchase = khaltiPayload.Remarks + "&UserId=" + khaltiPayload.UserId + "&VehicleNo=" + khaltiPayload.VehicleNo + "&To=" + khalti.TradeName;
                    var payload = new
                    {
                        return_url = khalti.RURL,
                        website_url = khalti.URL,
                        amount = FinalAmnt,
                        purchase_order_id = purcahse_id,
                        modes = new List<string> { "KHALTI" },
                        purchase_order_name = Purchase,
                        customer_info = new
                        {
                            name = khalti.TradeName,
                            email = khalti.Email,
                            phone = khalti.ContactNo.ToString(),
                        }
                    };

                    var jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Authorization", "key " + khalti.SecretKey);

                    var response = await client.PostAsync(url, content);
                    var responseContent = await response.Content.ReadAsStringAsync();




                    JToken jtoken = JToken.Parse(responseContent);
                    if (jtoken is JArray)
                    {
                        JArray jArray = (JArray)jtoken;
                        return Content(jArray.ToString(), "application/json");
                    }
                    else if (jtoken is JObject)
                    {
                        JObject jObject = (JObject)jtoken;
                        return Content(jObject.ToString(), "application/json");
                    }
                    else
                        return Content("Invalid JSON format", "application/json");
                }



                catch (JsonReaderException jsonEx)
                {
                    return Content($"{{\"Error\": \"JSON Parsing Error\", \"details\": \"{jsonEx.Message}\"}}", "application/json");
                }
                catch (Exception ex)
                {
                    return Content($"{{\"Error\": \"An unexpected error occurred\", \"details\": \"{ex.Message}\"}}", "application/json");
                }
            }
        }


        //This is my Return URL please create one return url on your site it is more easier while you create this on frontend else after successfully inserting  Khalti Success Response on database then open frontend payment success view

        //public async Task<ActionResult> KhaltiVerification(string Initial, string txnId, string pidx, string To, string From, string amount, string CID, string SID, string mobile, string purchase_order_id, string purchase_order_name, string transaction_id)
        //{
        //}


    }
}
