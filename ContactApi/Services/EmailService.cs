using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;

namespace ContactApi.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendContactNotificationAsync(string name, string email, string subject, string message)
    {
        var apiKey = _configuration["EmailSettings:SendGridApiKey"];
        var receiverEmail = _configuration["EmailSettings:ReceiverEmail"];
        var senderEmail = _configuration["EmailSettings:SenderEmail"];

        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(senderEmail, "Portfolio Contact"),
            Subject = $"Portfolio Contact: {subject}",
            HtmlContent = $@"
                <h2>New Contact Form Submission</h2>
                <p><strong>Name:</strong> {name}</p>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Subject:</strong> {subject}</p>
                <p><strong>Message:</strong></p>
                <p>{message}</p>
                <hr/>
                <p><small>Sent from your portfolio website</small></p>
            "
        };
        msg.AddTo(new EmailAddress(receiverEmail));

        var response = await client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"SendGrid error: {response.StatusCode}");
        }
    }
}