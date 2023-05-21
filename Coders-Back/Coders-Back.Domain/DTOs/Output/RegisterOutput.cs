using System.Text.Json.Serialization;

namespace Coders_Back.Domain.DTOs.Output;

public class RegisterOutput
{
    public bool Success { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }
}