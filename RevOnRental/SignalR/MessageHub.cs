using Microsoft.AspNetCore.SignalR;
using RevOnRental.Domain.Constants;
using RevOnRental.SignalR.Interfaces;

namespace RevOnRental.SignalR
{
 
        public class MessageHub : Hub
        {
            private readonly IUserConnectionManager _userConnectionManager;

            public MessageHub(IUserConnectionManager userConnectionManager)
            {
                _userConnectionManager = userConnectionManager;
            }

            /// <summary>
            /// Connect and Keep user in dictionary with signalId
            /// </summary>
            public override async Task OnConnectedAsync()
            {
                await Clients.All.SendAsync("onConnectedAsync", $"{Context.ConnectionId}");
                Guid userId = Guid.Parse(Context.User.Claims.FirstOrDefault(x => x.Type == AuthConstants.JwtId).Value);
                _userConnectionManager.KeepUserConnection(userId, Context.ConnectionId);
                await Clients.All.SendAsync("onConnectedAsync", $"{Context.ConnectionId}");
                await Clients.All.SendAsync("onUserActive", userId);
            }

            /// <summary>
            /// Called when a connection with the hub is terminated.
            /// </summary>
            public override async Task OnDisconnectedAsync(Exception exception)
            {
                Guid userId = Guid.Parse(Context.User.Claims.FirstOrDefault(x => x.Type == AuthConstants.JwtId).Value);
                //get the connectionId
                var connectionId = Context.ConnectionId;
                _userConnectionManager.RemoveUserConnection(connectionId);
                var value = await Task.FromResult(0);
                await Clients.All.SendAsync("onUserInActive", userId);
            }

            /// <summary>
            /// get all online user
            /// </summary>
            /// <returns></returns>
            public async Task GetOnlineUsers()
            {
                var userList = _userConnectionManager.OnlineUsers;
                await Clients.All.SendAsync("OnlineUsers", userList);
            }
        }
    }

