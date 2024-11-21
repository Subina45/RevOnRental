using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.Interfaces.SignalR
{
    public interface IUserConnectionManager
    {
        void KeepUserConnection(Guid userId, string connectionId);

        void RemoveUserConnection(string connectionId);

        List<string> GetUserConnections(Guid userId);

        List<string> GetUserConnections(List<Guid> userIdList);

        IEnumerable<Guid> OnlineUsers { get; }
    }
}
