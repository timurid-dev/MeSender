using MeSender.Messages.Models;
using Microsoft.EntityFrameworkCore;

namespace MeSender.Messages.Data;

public sealed class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    public DbSet<Message> Messages { get; init; }
}
