using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace personal_project.Hubs
{
  public class TestHub : Hub
  {
    private static RoomManager roomManager = new RoomManager();

    public override Task OnConnectedAsync()
    {
      return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
      roomManager.DeleteRoom(Context.ConnectionId);
      _ = NotifyRoomInfoAsync(false);
      return base.OnDisconnectedAsync(exception);
    }

    public async Task CreateRoom(string name)
    {
      RoomInfo roomInfo = roomManager.CreateRoom(Context.ConnectionId, name);
      if (roomInfo != null)
      {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomInfo.RoomId);
        await Clients.Caller.SendAsync("created", roomInfo.RoomId);
        await NotifyRoomInfoAsync(false);
      }
      else
      {
        await Clients.Caller.SendAsync("error", "error occurred when creating a new room.");
      }
    }

    public async Task Join(string roomId)
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
      await Clients.Caller.SendAsync("joined", roomId);
      await Clients.Group(roomId).SendAsync("ready");

      //
      if (int.TryParse(roomId, out int id))
      {
        roomManager.DeleteRoom(id);
        await NotifyRoomInfoAsync(false);
      }
    }

    public async Task LeaveRoom(string roomId)
    {
      await Clients.Group(roomId).SendAsync("bye");
    }

    public async Task GetRoomInfo()
    {
      await NotifyRoomInfoAsync(true);
    }

    public async Task SendMessage(string roomId, object message)
    {
      await Clients.OthersInGroup(roomId).SendAsync("message", message);
    }

    public async Task NotifyRoomInfoAsync(bool notifyOnlyCaller)
    {
      List<RoomInfo> roomInfos = roomManager.GetAllRoomInfo();
      var list = from room in roomInfos
                 select new
                 {
                   RoomId = room.RoomId,
                   Name = room.Name,
                   Button = "<button class=\"joinButton\">Join!</button>"
                 };
      var data = JsonSerializer.Serialize(list);

      if (notifyOnlyCaller)
      {
        await Clients.Caller.SendAsync("updateRoom", data);
      }
      else
      {
        await Clients.All.SendAsync("updateRoom", data);
      }
    }


  }

  public class RoomManager
  {
    private int nextRommId;
    private ConcurrentDictionary<int, RoomInfo> rooms;

    public RoomManager()
    {
      nextRommId = 1;
      rooms = new ConcurrentDictionary<int, RoomInfo>();
    }

    public RoomInfo CreateRoom(string connectionId, string name)
    {
      rooms.TryRemove(nextRommId, out _);

      // create new room info
      var roomInfo = new RoomInfo
      {
        RoomId = nextRommId.ToString(),
        Name = name,
        HostConnectionId = connectionId
      };

      bool result = rooms.TryAdd(nextRommId, roomInfo);

      if (result)
      {
        nextRommId++;
        return roomInfo;
      }
      else
      {
        return null;
      }
    }

    public void DeleteRoom(int roomId)
    {
      rooms.TryRemove(roomId, out _);
    }

    public void DeleteRoom(string connectionId)
    {
      int? correspondingRoomId = null;
      foreach (var pair in rooms)
      {
        if (pair.Value.HostConnectionId.Equals(connectionId))
        {
          correspondingRoomId = pair.Key;
        }
      }

      if (correspondingRoomId.HasValue)
      {
        rooms.TryRemove(correspondingRoomId.Value, out _);
      }
    }

    public List<RoomInfo> GetAllRoomInfo()
    {
      return rooms.Values.ToList();
    }
  }

  public class RoomInfo
  {
    public string? RoomId { get; set; }
    public string? Name { get; set; }
    public string? HostConnectionId { get; set; }
  }
}