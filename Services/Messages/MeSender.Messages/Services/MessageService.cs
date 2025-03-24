using MeSender.Messages.Data;
using MeSender.Messages.Models;
using Microsoft.EntityFrameworkCore;

namespace MeSender.Messages.Services;

internal sealed class MessageService(ChatDbContext context, TimeProvider timeProvider) : IMessageService
{
    public async Task SendMessageAsync(Guid messageId, string text, CancellationToken cancellationToken)
    {
        var message = await context.Messages.FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);

        if (message != null)
        {
            message.Text = text;
            message.UpdateTimestamp = timeProvider.GetUtcNow();
        }
        else
        {
            message = new Message
            {
                Id = messageId,
                Text = text,
                CreateTimestamp = timeProvider.GetUtcNow(),
            };
            context.Messages.Add(message);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<MessageDto>> ListMessageAsync(CancellationToken cancellationToken)
    {
        return await context.Messages
            .Select(x => new MessageDto
            {
                Id = x.Id,
                Text = x.Text,
                CreateTimestamp = x.CreateTimestamp,
                UpdateTimestamp = x.UpdateTimestamp,
            })
            .OrderBy(x => x.CreateTimestamp)
            .ToListAsync(cancellationToken);
    }
}
