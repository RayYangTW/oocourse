using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace personal_project.Hubs
{
  public class VideoChatHub : Hub
  {
    private static readonly ConcurrentDictionary<string, string> ActiveConnections = new ConcurrentDictionary<string, string>();

    public override Task OnConnectedAsync()
    {
      ActiveConnections.TryAdd(Context.ConnectionId, "");
      Console.WriteLine($"New Connection {Context.ConnectionId}");
      return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
      ActiveConnections.TryRemove(Context.ConnectionId, out _);
      Console.WriteLine($"Removed: {Context.ConnectionId}");
      return base.OnDisconnectedAsync(exception);
    }

    private bool IsValidConnection(string connectionId)
    {
      return ActiveConnections.ContainsKey(connectionId);
    }

    public async Task Offer(string connectionId, string sdpOffer, string username)
    {
      Console.WriteLine(sdpOffer);
      Console.WriteLine(username);

      if (IsValidConnection(connectionId))
      {
        await Clients.Client(connectionId).SendAsync("ReceiveOffer", Context.ConnectionId, sdpOffer, username);
        Console.WriteLine("sent");
      }
      else
      {
        Console.WriteLine("Error!");
      }
    }

    public async Task Answer(string connectionId, string sdpAnswer, string username)
    {
      await Clients.Client(connectionId).SendAsync("ReceiveAnswer", Context.ConnectionId, sdpAnswer, username);
    }

    public async Task SendIceCandidate(string connectionId, string candidate)
    {
      await Clients.Client(connectionId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
    }

  }
}