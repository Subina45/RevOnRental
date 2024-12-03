using MediatR;
using Microsoft.EntityFrameworkCore;
using RevOnRental.Application.Interfaces;
using RevOnRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Services.Payments.Command
{
    public class CompletePaymentCommand : IRequest<bool>
    {
        public string TransactionId { get; set; }
        public string tidx { get; set; }
        public string pidx { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
    public class CompletePaymentCommandHandler : IRequestHandler<CompletePaymentCommand, bool>
    {
        private readonly IAppDbContext _context;
        private readonly IMediator _mediator;

        public CompletePaymentCommandHandler(IAppDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<bool> Handle(CompletePaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == request.TransactionId, cancellationToken);

            if (payment == null)
            {
                return false;
            }

            payment.PaymentStatus = PaymentStatus.Completed;
            payment.TransactionId = request.TransactionId;
            payment.tidx = request.tidx;
            payment.pidx = request.pidx;
            payment.UpdatedDate = request.UpdatedDate;


            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}
