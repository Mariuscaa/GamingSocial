using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace HIOF.GamingSocial.GUI.Services;

public class ChatService
{
    private readonly NavigationManager _navigationManager;
    private HubConnection? _hubConnection;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
    public event Action<string, string> OnMessageReceived;
    public event Action<string, string, string> OnGroupMessageReceived;

    public ChatService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public async Task StartAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri("/chat"))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, string, string>("ReceiveMessage", (user, message, timestamp) =>
        {
            OnMessageReceived?.Invoke(user, $"{message}");
        });

        _hubConnection.On<string, string, string, string>("ReceiveGroupMessage", (roomName, user, message, timestamp) =>
        {
            OnGroupMessageReceived?.Invoke(roomName, user, $"{timestamp}: {message}");
        });
        await _hubConnection.StartAsync();
    }


    public async Task JoinRoom(string roomName)
    {
        if (IsConnected)
        {
            await _hubConnection.SendAsync("JoinRoom", roomName);
        }
    }

    public async Task LeaveRoom(string roomName)
    {
        if (IsConnected)
        {
            await _hubConnection.SendAsync("LeaveRoom", roomName);
        }
    }

    public async Task SendMessageToGroup(string roomName, string user, string message, string timestamp)
    {
        if (IsConnected)
        {
            await _hubConnection.SendAsync("SendMessageToGroup", roomName, user, message, timestamp);
        }
    }


    public async Task ConnectUser(string userName)
    {
        if (IsConnected)
        {
            await _hubConnection.SendAsync("ConnectUser", userName);
        }
    }

    public async Task SendMessageToUser(string targetUserName, string user, string message, string timestamp)
    {
        if (IsConnected)
        {
            await _hubConnection.SendAsync("SendMessageToUser", targetUserName, user, message, timestamp);
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

}
