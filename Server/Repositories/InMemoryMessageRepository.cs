using BlazorWebAssemblySignalRApp.Server.Models;

namespace BlazorWebAssemblySignalRApp.Server.Repositories
{

    /// <summary>
    /// In-memory backed message storage.
    /// </summary>
    public class InMemoryMessageRepository : IMessageRepository
    {

        private readonly List<Message> _messages = new();

        public Task AddMessageAsync(string message, string user, string group)
        {
            var msg = new Message { MessageText = message, User = user, Group = group };
            _messages.Add(msg);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<Message>> GetMessagesAsync(string group)
        {
            return Task.FromResult((IReadOnlyCollection<Message>)_messages.Where(m => m.Group == group).ToArray());
        }

    }
}
