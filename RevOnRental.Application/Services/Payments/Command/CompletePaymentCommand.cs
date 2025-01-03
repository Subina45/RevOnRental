﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using RevOnRental.Application.Interfaces;
using RevOnRental.Application.Services.RentalBooking.Command;
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
        public int RentalId { get; set; }
        public string TransactionId { get; set; }
        public string tidx { get; set; }
        public string pidx { get; set; }
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
                .FirstOrDefaultAsync(p => p.RentalId == request.RentalId, cancellationToken);

            if (payment == null)
            {
                return false;
            }

            payment.PaymentStatus = PaymentStatus.Completed;
            payment.TransactionId = request.TransactionId;
            payment.tidx = request.tidx;
            payment.pidx = request.pidx;
            payment.UpdatedDate = DateTime.Now;
            await _mediator.Send(
                new ConfirmRentalCommand
                {
                     PaymentType=PaymentType.Online,
                      RentalId=request.RentalId,
                }
                );
            

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}
