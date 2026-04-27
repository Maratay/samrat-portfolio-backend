using Microsoft.AspNetCore.Mvc;
using ContactApi.Models;
using ContactApi.Data;
using ContactApi.Services;
using Microsoft.EntityFrameworkCore;

namespace ContactApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public ContactController(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendContact([FromBody] ContactFormDto formDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Save to database
        var contactMessage = new ContactMessage
        {
            Name = formDto.Name,
            Email = formDto.Email,
            Subject = formDto.Subject,
            Message = formDto.Message,
            CreatedAt = DateTime.UtcNow
        };

        _context.ContactMessages.Add(contactMessage);
        await _context.SaveChangesAsync();

        // Send email notification
        try
        {
            await _emailService.SendContactNotificationAsync(
                formDto.Name,
                formDto.Email,
                formDto.Subject,
                formDto.Message
            );
        }
        catch (Exception ex)
        {
            // Log error but don't fail the request
            Console.WriteLine($"Email sending failed: {ex.Message}");
        }

        return Ok(new { success = true, message = "Message sent successfully!" });
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages()
    {
        var messages = await _context.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return Ok(messages);
    }
}