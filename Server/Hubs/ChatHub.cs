using System.Threading.Tasks;
using BlazorWebAssemblySignalRApp.Server.Models;
using BlazorWebAssemblySignalRApp.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorWebAssemblySignalRApp.Server.Hubs
{
    public class ChatHub : Hub
    {

        public ChatHub(ChatDbContext dbContext)
        {
            _dbContext = dbContext;
            // TODO: move this elsewhere
            _dbContext.Database.EnsureCreated();
        }

        private readonly ChatDbContext _dbContext;

        private static readonly Dictionary<string, Group> _groups = new Dictionary<string, Group>();
        private static readonly List<Message> _messages = new List<Message>();

        private List<GroupDTO> GroupDTOs => _groups.Values.Select(g => g.ToDTO()).ToList();

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

            if (!_groups.TryGetValue(groupName, out var group)) 
            {
                group = new Group { Name = groupName };
                _groups.Add(groupName, group);
                await Clients.All.SendAsync("RefreshGroups", GroupDTOs);
            } else if (group.Members.Count >= Constants.MaxMemberCount)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "The group is full");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            group.Members.Add(user);

            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, $"has joined the group {groupName}.");

            var groupMessages = _messages.Where(m => m.Group == groupName).ToArray();
            foreach (var msg in groupMessages)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", msg.User, msg.MessageText);
            }

        }

        public async Task RemoveFromGroup(string user, string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, $"has left the group {groupName}.");
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("RefreshGroups", GroupDTOs);
            await base.OnConnectedAsync();
        }

    }

}
