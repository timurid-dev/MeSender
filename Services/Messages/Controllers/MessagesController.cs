using MeSender.Messages.WebApi.Data;
using MeSender.Messages.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeSender.Messages.WebApi.Controllers;

[ApiController]
[Route("api/messages")]
public sealed class MessagesController(ChatDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Message>>> GetMessagesAsync()
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

        context.Messages.Add(message);
        await context.SaveChangesAsync(HttpContext.RequestAborted);

        return CreatedAtAction(nameof(GetMessagesAsync), new
        {
            id = message.Id,
        }, message);
    }
}
