using System.Net;
using System.Net.Mail;
using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Coders_Back.Domain.ExternalServices;

public class EmailServiceProvider : IEmailServiceProvider
{
    private readonly ILogger<EmailServiceProvider> _logger;

    public EmailServiceProvider(IConfiguration configuration, ILogger<EmailServiceProvider> logger)
    {
        _logger = logger;
        _password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? configuration["EmailProvider:Password"];
        _email = Environment.GetEnvironmentVariable("SMTP_EMAIL") ?? configuration["EmailProvider:Email"];
    }
    
    private static string? _email;
    private const string Host = "smtp.office365.com";
    private const int Port = 587;
    private static string? _password;
    
    public void SendEmail(SendEmailInput input)
    {
        if (_password is null || _email is null)
            _logger.LogError("SMTP password or email not found. If running for development," +
                             " check appsettings.json or docker-compose.yml and correct");
        
        var message = new MailMessage
        {
            Body = input.Body.Invoke(),
            From = new MailAddress(_email),
            IsBodyHtml = input.IsBodyHtml,
            Subject = input.Subject
        };
        
        message.From = new MailAddress(_email);
        
        foreach (var recipient in input.Recipients)
        {
            message.To.Add(recipient);
        }
        
        var smtpClient = new SmtpClient
        {
            Credentials = new NetworkCredential(_email, _password),
            EnableSsl = true, 
            Host = Host,
            Port = Port,
            UseDefaultCredentials = input.UseDefaultCredentials
        };

        smtpClient.Send(message);
    }
}