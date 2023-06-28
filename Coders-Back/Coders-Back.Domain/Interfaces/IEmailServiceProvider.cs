using Coders_Back.Domain.DTOs.Input;

namespace Coders_Back.Domain.Interfaces;

public interface IEmailServiceProvider
{
    void SendEmail(SendEmailInput input);
}