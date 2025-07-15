using MassTransit;
using MeSender.Identity.Events.Models;
using MeSender.Messages.Services;

namespace MeSender.Messages.WebApi.Models;

internal sealed class UserLoggedInConsumer(IMessageService messageService) : IConsumer<UserLoggedInDto>
{
    public async Task Consume(ConsumeContext<UserLoggedInDto> context)
    {
        var email = context.Message.Email;
        await messageService.SendMessageAsync(
            Guid.NewGuid(),
            $"Привет, {email}!",
            context.CancellationToken);
    }
}
