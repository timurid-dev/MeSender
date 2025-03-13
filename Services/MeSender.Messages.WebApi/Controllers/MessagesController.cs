using MeSender.Messages.Models;
using MeSender.Messages.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeSender.Messages.WebApi.Controllers;

[ApiController]
[Route("api/messages/")]
public sealed class MessagesController(IDataHandler dataHandler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ICollection<Messages.Models.Message>>> GetMessagesAsync()
    {
        return await dataHandler.SendDataAsync(HttpContext.RequestAborted);
    }

    [HttpPut("{messageId:guid}")]
    public async Task<ActionResult> PutMessageAsync(Guid messageId, [FromBody] MessageDto messageDto)
    {
        if (string.IsNullOrEmpty(messageDto.Text))
        {
            return BadRequest("Message content cannot be empty.");
        }

        await dataHandler.ReceiveDataAsync(messageId, messageDto.Text, HttpContext.RequestAborted);

        return NoContent();
    }
}
