﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BlazorWebAssemblySignalRApp.Server.Hubs
{
    public class ChatHub : Hub
    {

        private static readonly HashSet<string> _groups = new HashSet<string>();
        private static readonly List<Message> _messages = new List<Message>();

        public async Task SendMessage(string user, string message)
        {
            _messages.Add(new Message { User = user, MessageText = message });
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task AddToGroup(string user, string groupName)
        {

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            if (!_groups.Contains(groupName))
            {
                _groups.Add(groupName);
                await Clients.All.SendAsync("RefreshGroups", _groups);
            }

            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, $"has joined the group {groupName}.");
        }

        public override async Task OnConnectedAsync()
        {
            foreach (var message in _messages)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", message.User, message.MessageText);
            }
            await base.OnConnectedAsync();
        }
    }

    public class Message
    {
        public string Group { get; init; }
        public string MessageText { get; init; }
        public string User { get; init; }
        public DateTime Timestamp { get; } = DateTime.Now;
    }
}
