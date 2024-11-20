using RevOnRental.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Interfaces
{
    public interface IRentalBooking
    {
        Task<List<RentalDto>> GetRentalsByBusinessIdQuery(int businessId);
        Task<List<RentalDto>> GetUserBookingHistoryQuery(int userId);
    }
}
