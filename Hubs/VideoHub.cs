using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace personal_project.Hubs
{
  public class VideoHub : Hub
  {
    // 加入房間
    public async Task JoinRoom(string roomId, string userId)
    {
      VideoHub.list.Add(Context.ConnectionId, userId);
      await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
      await Clients.Group(roomId).SendAsync("user-connected", userId);
    }

    // 離線
    public override Task OnDisconnectedAsync(Exception? exception)
    {
      Clients.All.SendAsync("user-disconnected", VideoHub.list[Context.ConnectionId]);
      return base.OnDisconnectedAsync(exception);
    }

    // For setting user list
    public static IDictionary<string, string> list = new Dictionary<string, string>();
  }

}