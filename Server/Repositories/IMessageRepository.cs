using BlazorWebAssemblySignalRApp.Server.Models;

namespace BlazorWebAssemblySignalRApp.Server.Repositories
{

    /// <summary>
    /// Persists chat messages.
    /// </summary>
    public interface IMessageRepository
    {
        Task AddMessageAsync(string message, string user, string group);
        Task<IReadOnlyCollection<Message>> GetMessagesAsync(string group);
    }

}
