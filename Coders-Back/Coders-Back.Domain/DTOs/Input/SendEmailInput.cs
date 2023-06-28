namespace Coders_Back.Domain.DTOs.Input;

public class SendEmailInput
{
    public string Subject { get; set; }
    public IEnumerable<string> Recipients { get; set; }
    public Func<string> Body { get; set; }
    public bool IsBodyHtml { get; set; } = false;
    public bool EnableSsl { get; set; } = true;
    public bool UseDefaultCredentials { get; set; } = false;
}