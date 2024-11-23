using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.SignalR.Interfaces
{
    public interface IUserConnectionManager
    {
        void KeepUserConnection(Guid userId, string connectionId);

        void RemoveUserConnection(string connectionId);

        HashSet<string> GetUserConnections(Guid userId);

        IEnumerable<Guid> OnlineUsers { get; }
    }
}
