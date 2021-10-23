using BlazorWebAssemblySignalRApp.Server.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace BlazorWebAssemblySignalRApp.Server.Hubs
{
    public class ChatHub : Hub
    {

        public ChatHub(IMessageRepository messages, IGroupRepository groups)
        {
            _messages = messages;
            _groups = groups;
        }

        private readonly IGroupRepository _groups;
        private readonly IMessageRepository _messages;

        public async Task SendMessage(string user, string message, string group)
        {
            await Clients.Group(group).SendAsync("ReceiveMessage", user, message);
            await _messages.AddMessageAsync(message, user, group);
        }

        public async Task AddToGroup(string user, string groupName)
        {

            if (!_groups.TryJoinGroup(user, groupName))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "The group is full");
                return;
            }

            await Clients.All.SendAsync("RefreshGroups", _groups.GroupDTOs);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, $"has joined the group {groupName}.");

            var groupMessages = await _messages.GetMessagesAsync(groupName);
            foreach (var msg in groupMessages)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", msg.User, msg.MessageText);
            }

        }

        public async Task RemoveFromGroup(string user, string groupName)
        {
            _groups.LeaveGroup(user, groupName);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, $"has left the group {groupName}.");
            await Clients.Caller.SendAsync("RefreshGroups", _groups.GroupDTOs);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("RefreshGroups", _groups.GroupDTOs);
            await base.OnConnectedAsync();
        }

    }

}
