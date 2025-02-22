using MeSender.Messages.WebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeSender.Messages.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController(ChatDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Models.Message>>> GetMessages()
    {
        return await context.Messages.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Models.Message>> PostMessage([FromBody] Models.Message message)
    {
        if (string.IsNullOrEmpty(message.Text))
            return BadRequest("Message content cannot be empty.");
        
        context.Messages.Add(message);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMessages), new { id = message.Id }, message);
    }
}