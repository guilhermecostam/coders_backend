using System.Text.Json.Serialization;
using Coders_Back.Domain.Enums;

namespace Coders_Back.Domain.DTOs.Output;

public class ProjectJoinRequestCreateOutput
{
    public bool Success { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public RequestCreateOutputError? Error { get; set; }
}