using Microsoft.EntityFrameworkCore;

namespace Message.Data;

public class ChatDbContext: DbContext
{
    public DbSet<Models.Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Data/Messages.db");
    }
}