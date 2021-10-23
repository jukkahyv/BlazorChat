using BlazorWebAssemblySignalRApp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebAssemblySignalRApp.Server.Repositories
{
    /// <summary>
    /// Database (EF Core) backed message storage.
    /// </summary>
    public class DatabaseMessageRepository : IMessageRepository
    {

        public DatabaseMessageRepository(ChatDbContext dbContext)
        {
            _dbContext = dbContext;
            // TODO: move this elsewhere
            _dbContext.Database.EnsureCreated();
        }

        private readonly ChatDbContext _dbContext;

        public async Task AddMessageAsync(string message, string user, string group)
        {
            var msg = new Message { User = user, MessageText = message, Group = group };
            await _dbContext.AddAsync(msg);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Message>> GetMessagesAsync(string group)
        {
            var messages = await _dbContext.Messages.Where(m => m.Group == group).ToArrayAsync();
            return messages;
        }

    }

}
