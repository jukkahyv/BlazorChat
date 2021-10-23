using BlazorWebAssemblySignalRApp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebAssemblySignalRApp.Server
{
    public class ChatDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set;}
    }
}
