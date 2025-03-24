using MeSender.Messages.Models;
using Microsoft.EntityFrameworkCore;

namespace MeSender.Messages.Data;

internal sealed class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    internal DbSet<Message> Messages { get; init; }
}
