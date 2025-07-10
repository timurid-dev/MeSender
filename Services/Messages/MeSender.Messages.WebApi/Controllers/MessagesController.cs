using MeSender.Messages.Services;
using MeSender.Messages.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeSender.Messages.WebApi.Controllers;

[ApiController]
[Route("api/messages/")]
[Authorize]
public sealed class MessagesController(IMessageService messageService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<MessageDto>>> ListMessagesAsync()
    {
        var messages = await messageService.ListMessageAsync(HttpContext.RequestAborted);
        return Ok(messages);
    }

    [HttpPut("{messageId:guid}")]
    public async Task<ActionResult> PutMessageAsync(Guid messageId, [FromBody] MessageDto messageDto)
    {
        if (string.IsNullOrEmpty(messageDto.Text))
        {
            return BadRequest("Message content cannot be empty.");
        }

        await messageService.SendMessageAsync(messageId, messageDto.Text, HttpContext.RequestAborted);

        return NoContent();
    }
}
