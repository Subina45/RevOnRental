using RevOnRental.Domain.Common;
using RevOnRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Domain.Models
{
    public class Payment : IBaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public int BusinessId { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string tidx { get; set; }
        public string pidx { get; set; }
        public string TransactionId { get; set; }


        public User User { get; set; }
        public Vehicle Vehicle { get; set; }
        public Business Business { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set ; }
    }
}
