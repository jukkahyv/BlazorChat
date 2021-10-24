using BlazorWebAssemblySignalRApp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorWebAssemblySignalRApp.Server
{
    public class ChatDbContext : DbContext
    {

        public ChatDbContext() { }

        public ChatDbContext(DbContextOptions<ChatDbContext> dbOptions)
            : base(dbOptions)
        {

        }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>().HasPartitionKey(m => m.Group);
            base.OnModelCreating(modelBuilder);
        }

    }
}
