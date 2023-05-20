using System.Text.Json.Serialization;
using Coders_Back.Domain.Enums;

namespace Coders_Back.Domain.DTOs.Output;

public class LoginOutput
{
    public bool Success { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }
    public LoginStatusOutput Status { get; set; }
}