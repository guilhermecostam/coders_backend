using System.Net;
using System.Net.Mail;
using Coders_Back.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Coders_Back.Domain.ExternalServices;

public class EmailServiceProvider : IEmailServiceProvider
{
    public EmailServiceProvider(IConfiguration configuration)
    {
        _pass = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? configuration["EmailProvider:Password"];
    }
    
    private const string Email = "codersproject@outlook.com";
    private const string Host = "smtp.office365.com";
    private const int Port = 587;
    private static string? _pass;
    
    public void SendEmail(string subject, IEnumerable<string> recipients,Func<string> body, bool isBodyHtml = false, bool enableSsl = true, bool useDefaultCredentials = false)
    {
        
        var message = new MailMessage
        {
            Body = body.Invoke(),
            From = new MailAddress(Email),
            IsBodyHtml = isBodyHtml,
            Subject = subject
        };
        
        message.From = new MailAddress(Email);
        
        foreach (var recipient in recipients)
        {
            message.To.Add(recipient);
        }
        
        var smtpClient = new SmtpClient
        {
            Credentials = new NetworkCredential(Email, _pass),
            EnableSsl = enableSsl,
            Host = Host,
            Port = Port,
            UseDefaultCredentials = useDefaultCredentials
        };

        smtpClient.Send(message);
    }
}