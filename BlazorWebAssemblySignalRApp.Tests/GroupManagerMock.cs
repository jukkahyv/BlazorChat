using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWebAssemblySignalRApp.Tests
{
    public class GroupManagerMock : IGroupManager
    {

        private readonly Dictionary<string, List<string>> _connectionsInGroups = new();

        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> ConnectionsInGroups
            => _connectionsInGroups.ToDictionary(c => c.Key, c => (IReadOnlyCollection<string>)c.Value);

        public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            if (!_connectionsInGroups.TryGetValue(connectionId, out var list))
            {
                list = new List<string>();
                _connectionsInGroups.Add(connectionId, list);
            }
            list.Add(groupName);
            return Task.CompletedTask;
        }

        public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
        {
            if (!_connectionsInGroups.TryGetValue(connectionId, out var list))
            {
                list = new List<string>();
                _connectionsInGroups.Add(connectionId, list);
            }
            list.Remove(groupName);
            return Task.CompletedTask;
        }
    }
}
