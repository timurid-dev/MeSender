using MeSender.Messages.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeSender.Messages.Models;

public sealed class DataHandler(ChatDbContext context) : IDataHandler
{
    public async Task ReceiveDataAsync(Guid messageId, string text, CancellationToken cancellationToken)
    {
        var message = await context.Messages.FirstOrDefaultAsync(
            x => x.Id == messageId,
            cancellationToken: cancellationToken);

        if (message != null)
        {
            message.Text = text;
            message.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            message = new Message
            {
                Text = text,
                CreatedAt = DateTime.UtcNow,
            };
            context.Messages.Add(message);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ActionResult<ICollection<Message>>> SendDataAsync(CancellationToken cancellationToken)
    {
        return await context.Messages.ToListAsync(cancellationToken: cancellationToken);
    }
}
