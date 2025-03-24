using MeSender.Messages.Models;

namespace MeSender.Messages.Services;

public interface IMessageService
{
    Task SendMessageAsync(Guid messageId, string text, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<MessageDto>> ListMessageAsync(CancellationToken cancellationToken);
}
