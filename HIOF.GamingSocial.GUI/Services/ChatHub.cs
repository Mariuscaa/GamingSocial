using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace HIOF.GamingSocial.GUI.Services;

public class ChatHub : Hub
{

    private static readonly ConcurrentDictionary<string, string> _userConnectionMapping = new ConcurrentDictionary<string, string>();

    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {roomName}.");
    }

    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }

    public async Task SendMessageToGroup(string roomName, string user, string message, string timestamp)
    {
        await Clients.Group(roomName).SendAsync("ReceiveGroupMessage", user, message, timestamp);

    }
    public async Task ConnectUser(string userName)
    {
        var connectionId = Context.ConnectionId;
        _userConnectionMapping.TryAdd(userName, connectionId);
    }

    public override async Task OnConnectedAsync()
    {

        var userName = Context.User.Identity.Name;
        var connectionId = Context.ConnectionId;

        if (!string.IsNullOrEmpty(userName))
        {
            _userConnectionMapping.TryAdd(userName, connectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        string connectionId = Context.ConnectionId;
        var user = _userConnectionMapping.FirstOrDefault(x => x.Value == connectionId).Key;

        if (user != null)
        {
            _userConnectionMapping.TryRemove(user, out _);
        }

        await base.OnDisconnectedAsync(exception);
    }


    public async Task SendMessageToUser(string targetUserName, string user, string message, string timestamp)
    {
        _userConnectionMapping.AddOrUpdate(user, Context.ConnectionId, (key, oldValue) => Context.ConnectionId);


        if (_userConnectionMapping.TryGetValue(targetUserName, out string connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", user, message, timestamp);
        }
    }
}
