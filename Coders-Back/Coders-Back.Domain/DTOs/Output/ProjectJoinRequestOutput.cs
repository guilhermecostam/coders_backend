using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Enums;

namespace Coders_Back.Domain.DTOs.Output;

public class ProjectJoinRequestOutput
{
    public ProjectJoinRequestOutput()
    {
    }

    public ProjectJoinRequestOutput(ProjectJoinRequest input)
    {
        Id = input.Id;
        ProjectId = input.ProjectId;
        UserId = input.UserId;
        Status = input.Status;
    }
    
    public Guid Id { get; set; }
    public RequestStatus Status { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
}