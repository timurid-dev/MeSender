using MeSender.Messages.Data;
using MeSender.Messages.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeSender.Messages.WebApi.Controllers;

[ApiController]
[Route("api/messages/")]
public sealed class MessagesController(ChatDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Messages.Models.Message>>> GetMessagesAsync()
    {
        return await context.Messages.ToListAsync(cancellationToken: HttpContext.RequestAborted);
    }

    [HttpPut("{messageId:guid}")]
    public async Task<ActionResult> PutMessageAsync(Guid messageId, [FromBody] MessageDto messageDto)
    {
        if (string.IsNullOrEmpty(messageDto.Text))
        {
            return BadRequest("Message content cannot be empty.");
        }

        var ctoken = HttpContext.RequestAborted;

        var existMessage = await context.Messages.FirstOrDefaultAsync(
            x => x.Id == messageId,
            cancellationToken: ctoken);

        var messageMapped = new Messages.Models.Message();
        if (existMessage != null)
        {
            messageMapped.Text = messageDto.Text;
            messageMapped.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            messageMapped.Id = messageDto.Id;
            messageMapped.Text = messageDto.Text;
            messageMapped.CreatedAt = DateTime.UtcNow;
        }

        context.Messages.Add(messageMapped);
        await context.SaveChangesAsync(ctoken);

        return CreatedAtAction(nameof(GetMessagesAsync), messageDto);
    }
}
