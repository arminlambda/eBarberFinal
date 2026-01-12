using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using eBarber.Data;
using eBarber.Models;

namespace eBarber.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MessagesApiController : ControllerBase
{
    private readonly BarberContext _context;

    public MessagesApiController(BarberContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult> SendMessage([FromBody] MessageDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var message = new Message
        {
            SenderId = userId!,
            ReceiverId = null,
            Content = dto.Content,
            SentAt = DateTime.Now,
            MessageType = "Text"
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Sporočilo poslano" });
    }
}

public class MessageDto
{
    public string Content { get; set; } = string.Empty;
}