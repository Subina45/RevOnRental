using RevOnRental.Application.Interfaces.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Infrastructure.SignalR
{
        public class UserConnectionManager : IUserConnectionManager
        {
            private static Dictionary<Guid, HashSet<string>> userConnectionMap = new Dictionary<Guid, HashSet<string>>();
            private static string userConnectionMapLocker = string.Empty;

            public void KeepUserConnection(Guid userId, string connectionId)
            {
                lock (userConnectionMapLocker)
                {
                    if (!userConnectionMap.ContainsKey(userId))
                    {
                        userConnectionMap[userId] = new HashSet<string>();
                    }
                    userConnectionMap[userId].Add(connectionId);
                }
            }

            public void RemoveUserConnection(string connectionId)
            {
                //Remove the connectionId of user
                lock (userConnectionMapLocker)
                {
                    foreach (var userId in userConnectionMap.Keys)
                    {
                        if (userConnectionMap.ContainsKey(userId))
                        {
                            if (userConnectionMap[userId].Contains(connectionId))
                            {
                                userConnectionMap[userId].Remove(connectionId);
                                break;
                            }
                        }
                    }
                }
            }

            public List<string> GetUserConnections(Guid userId)
            {
                if (!userConnectionMap.Any())
                {
                    return new() { };
                }

                List<string> connectionIds = new() { };
                lock (userConnectionMap)
                {
                    connectionIds = userConnectionMap
                                             .Where(x => x.Key == userId)
                                             .SelectMany(x => x.Value)
                                             .ToList();
                }
                return connectionIds;
            }

            public List<string> GetUserConnections(List<Guid> userIdList)
            {
                if (!userConnectionMap.Any())
                {
                    return new() { };
                }

                List<string> connectionIds = new() { };
                lock (userConnectionMap)
                {
                    connectionIds = userConnectionMap
                                             .Where(x => userIdList.Contains(x.Key))
                                             .SelectMany(x => x.Value)
                                             .ToList();
                }
                return connectionIds;
            }

            public IEnumerable<Guid> OnlineUsers
            { get { return userConnectionMap.Keys; } }
        }
}
