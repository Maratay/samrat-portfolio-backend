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
        // Read values
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = _configuration["EmailSettings:SmtpPort"];
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var senderPassword = _configuration["EmailSettings:SenderPassword"];
        var receiverEmail = _configuration["EmailSettings:ReceiverEmail"];

        // DEBUG: Print EVERYTHING
        Console.WriteLine("========== EMAIL DEBUG INFO ==========");
        Console.WriteLine($"SmtpServer: '{smtpServer}'");
        Console.WriteLine($"SmtpPort: '{smtpPort}'");
        Console.WriteLine($"SenderEmail: '{senderEmail}'");
        Console.WriteLine($"SenderPassword: '{senderPassword?.Substring(0, Math.Min(4, senderPassword?.Length ?? 0))}...' (length: {senderPassword?.Length ?? 0})");
        Console.WriteLine($"ReceiverEmail: '{receiverEmail}'");
        Console.WriteLine("=======================================");

        // Check for nulls
        if (string.IsNullOrEmpty(senderEmail))
        {
            throw new Exception("SenderEmail is empty! Check appsettings.json");
        }
        if (string.IsNullOrEmpty(receiverEmail))
        {
            throw new Exception("ReceiverEmail is empty! Check appsettings.json");
        }
        if (string.IsNullOrEmpty(senderPassword))
        {
            throw new Exception("SenderPassword is empty! Did you set the App Password?");
        }

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail),
            Subject = $"Portfolio Contact: {subject}",
            Body = $@"
                <h2>New Contact Form Submission</h2>
                <p><strong>Name:</strong> {name}</p>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Subject:</strong> {subject}</p>
                <p><strong>Message:</strong></p>
                <p>{message}</p>
                <hr/>
                <p><small>Sent from your portfolio website</small></p>
            ",
            IsBodyHtml = true
        };

        mailMessage.To.Add(receiverEmail);

        using var smtpClient = new SmtpClient(smtpServer, int.Parse(smtpPort ?? "587"))
        {
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
        Console.WriteLine("✅ Email sent successfully!");
    }
}