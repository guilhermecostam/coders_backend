namespace Coders_Back.Domain.Interfaces;

public interface IEmailServiceProvider
{
    void SendEmail(string subject, IEnumerable<string> recipients,Func<string> body, bool isBodyHtml = false, bool enableSsl = true, bool useDefaultCredentials = false);
}