using MeSender.Messages.Data;
using MeSender.Messages.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeSender.Messages.WebApi.Controllers;

[ApiController]
[Route("api/messages")]
public sealed class MessagesController(ChatDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Messages.Models.Message>>> GetMessagesAsync()
    {
        return await context.Messages.ToListAsync(cancellationToken: HttpContext.RequestAborted);
    }

    [HttpPost]
    public async Task<ActionResult<Message>> PostMessageAsync([FromBody] Message message)
    {
        if (string.IsNullOrEmpty(message.Text))
        {
            return BadRequest("Message content cannot be empty.");
        }

        var messageMapped = new Messages.Models.Message()
        {
            Text = message.Text,
        };

        context.Messages.Add(messageMapped);
        await context.SaveChangesAsync(HttpContext.RequestAborted);

        return CreatedAtAction(nameof(GetMessagesAsync), message);
    }
}
