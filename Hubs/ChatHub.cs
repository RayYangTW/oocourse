using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace personal_project.Hubs
{
  public class ChatHub : Hub
  {
    public async Task SendMessage(string user, string message)
    {
      var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
      await Clients.All.SendAsync("ReceiveMessage", user, message, time);
    }
  }
}