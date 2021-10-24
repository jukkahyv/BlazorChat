using BlazorWebAssemblySignalRApp.Server.Repositories;
using BlazorWebAssemblySignalRApp.Shared;
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
        /// <summary>
        /// Maps connection ID to user name.
        /// Not happy with this. We should use identity management instead.
        /// </summary>
        private readonly static Dictionary<string, string> _userMap = new();

        public async Task SendMessage(string user, string message, string group)
        {
            await Clients.Group(group).SendAsync(MessageNames.ReceiveMessage, user, message);
            await _messages.AddMessageAsync(message, user, group);
        }

        public async Task AddToGroup(string user, string groupName)
        {

            if (!_groups.TryJoinGroup(user, groupName))
            {
                await Clients.Caller.SendAsync(MessageNames.ReceiveMessage, "System", "The group is full");
                return;
            }

            _userMap[Context.ConnectionId] = user;

            await Clients.All.SendAsync(MessageNames.RefreshGroups, _groups.GroupDTOs);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync(MessageNames.ReceiveMessage, user, $"has joined the group {groupName}.");

            var groupMessages = await _messages.GetMessagesAsync(groupName);
            foreach (var msg in groupMessages)
            {
                await Clients.Caller.SendAsync(MessageNames.ReceiveMessage, msg.User, msg.MessageText);
            }

        }

        public async Task RemoveFromGroup(string user, string groupName)
        {
            _groups.LeaveGroup(user, groupName);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync(MessageNames.ReceiveMessage, user, $"has left the group {groupName}.");
            await Clients.Caller.SendAsync(MessageNames.RefreshGroups, _groups.GroupDTOs);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync(MessageNames.RefreshGroups, _groups.GroupDTOs);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_userMap.TryGetValue(Context.ConnectionId, out var userName))
            {
                _groups.LeaveGroups(userName);
            }
            await Clients.Others.SendAsync(MessageNames.RefreshGroups, _groups.GroupDTOs);
            await base.OnDisconnectedAsync(exception);
        }

    }

}
