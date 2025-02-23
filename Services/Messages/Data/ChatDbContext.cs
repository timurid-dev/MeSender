using MeSender.Messages.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MeSender.Messages.WebApi.Data;

public sealed class ChatDbContext : DbContext
{
    public DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Data/Messages.db");
    }
}
