using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Dtos
{
    public class PaymentRequestDto
    {
        public string ReturnUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string Amount { get; set; }
        public string PurchaseRentalId { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public int BusinessId { get; set; }
    }
}
