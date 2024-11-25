using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Domain.Models.PaymentGateway
{
    public class KhaltiDTO
    {
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public string KhaltiUrl { get; set; }
        public string TradeName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string URL { get; set; }
        public string RURL { get; set; }
    }

    public class KhaltiPayload
    {
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public string VehicleNo { get; set; }
        public string Remarks { get; set; }
    }
}
