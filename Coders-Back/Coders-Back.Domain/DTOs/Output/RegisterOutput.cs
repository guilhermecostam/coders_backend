using Coders_Back.Domain.Enums;

namespace Coders_Back.Domain.DTOs.Output;

public class RegisterOutput
{
    public bool Success { get; set; }

    public RegisterStatusOutput Status { get; set; }
}