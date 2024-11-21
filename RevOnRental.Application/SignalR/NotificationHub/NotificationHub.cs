using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RevOnRental.Application.Interfaces.SignalR;
using RevOnRental.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevOnRental.Application.SignalR.NotificationHub
{
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public class NotificationHub : Hub
        {
            private readonly IUserConnectionManager _userConnectionManager;

            public NotificationHub(IUserConnectionManager userConnectionManager)
            {
                _userConnectionManager = userConnectionManager;
            }

            /// <summary>
            /// Connect and Keep user in dictionary with signalIdv
            /// </summary>
            public override async Task OnConnectedAsync()
            {
                await Clients.All.SendAsync("onConnectedAsync", $"{Context.ConnectionId}");
                Guid userId = Guid.Parse(Context.User.Claims.FirstOrDefault(x => x.Type == AuthConstants.JwtId).Value);
                _userConnectionManager.KeepUserConnection(userId, Context.ConnectionId);
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


    //public class NotificationHub : Hub
    //{
    //    // Send notification to a specific business
    //    public async Task SendBookingNotification(string businessId, string message)
    //    {
    //        await Clients.Group(businessId).SendAsync("ReceiveBookingNotification", message);
    //    }

    //    // Add a business to a SignalR group for targeted notifications
    //    public override async Task OnConnectedAsync()
    //    {
    //        var businessId = Context.GetHttpContext().Request.Query["businessId"];
    //        if (!string.IsNullOrEmpty(businessId))
    //        {
    //            await Groups.AddToGroupAsync(Context.ConnectionId, businessId);
    //        }
    //        await base.OnConnectedAsync();
    //    }

    //    // Remove a business from the SignalR group when disconnected
    //    public override async Task OnDisconnectedAsync(Exception? exception)
    //    {
    //        var businessId = Context.GetHttpContext().Request.Query["businessId"];
    //        if (!string.IsNullOrEmpty(businessId))
    //        {
    //            await Groups.RemoveFromGroupAsync(Context.ConnectionId, businessId);
    //        }
    //        await base.OnDisconnectedAsync(exception);
    //    }
    //}
}
