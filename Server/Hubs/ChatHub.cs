using BlazorWebAssemblySignalRApp.Server.Models;
using BlazorWebAssemblySignalRApp.Server.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace BlazorWebAssemblySignalRApp.Server.Hubs
{
    public class ChatHub : Hub
    {

        public ChatHub(ChatDbContext dbContext, IGroupRepository groups)
        {
            _dbContext = dbContext;
            // TODO: move this elsewhere
            _dbContext.Database.EnsureCreated();
            _groups = groups;
        }

        private readonly ChatDbContext _dbContext;
        private readonly IGroupRepository _groups;

        private static readonly List<Message> _messages = new List<Message>();

        public async Task SendMessage(string user, string message, string group)
        {
            var msg = new Message { User = user, MessageText = message, Group = group };
            _messages.Add(msg);
            await Clients.Group(group).SendAsync("ReceiveMessage", user, message);

            await _dbContext.AddAsync(msg);
            await _dbContext.SaveChangesAsync();

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

            var groupMessages = _messages.Where(m => m.Group == groupName).ToArray();
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
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("RefreshGroups", _groups.GroupDTOs);
            await base.OnConnectedAsync();
        }

    }

}
