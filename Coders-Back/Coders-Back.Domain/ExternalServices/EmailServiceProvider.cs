using System.Net;
using System.Net.Mail;
using Coders_Back.Domain.DTOs.Input;
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
    
    public void SendEmail(SendEmailInput input)
    {
        
        var message = new MailMessage
        {
            Body = input.Body.Invoke(),
            From = new MailAddress(Email),
            IsBodyHtml = input.IsBodyHtml,
            Subject = input.Subject
        };
        
        message.From = new MailAddress(Email);
        
        foreach (var recipient in input.Recipients)
        {
            message.To.Add(recipient);
        }
        
        var smtpClient = new SmtpClient
        {
            Credentials = new NetworkCredential(Email, _pass),
            EnableSsl = input.EnableSsl,
            Host = Host,
            Port = Port,
            UseDefaultCredentials = input.UseDefaultCredentials
        };

        smtpClient.Send(message);
    }
}