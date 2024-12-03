using MediatR;
using RevOnRental.Application.Interfaces;
using RevOnRental.Domain.Enums;
using RevOnRental.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Services.Payments.Command
{
    public class CreatePaymentCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public int BusinessId { get; set; }
        public int RentalId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; }
    }
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, int>
    {
        private readonly IAppDbContext _context;
        private readonly IMediator _mediator;

        public CreatePaymentCommandHandler(IAppDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<int> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var payment = new Payment
                {
                    UserId = request.UserId,
                    VehicleId = request.VehicleId,
                    BusinessId = request.BusinessId,
                    PaymentStatus = PaymentStatus.Pending,
                    PaymentType=PaymentType.Online,
                    TotalPrice = request.TotalPrice,
                    PaymentDate = request.PaymentDate,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync(cancellationToken);

                return payment.Id;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }

}
