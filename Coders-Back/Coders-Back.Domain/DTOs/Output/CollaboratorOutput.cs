using Coders_Back.Domain.Entities;

namespace Coders_Back.Domain.DTOs.Output;

public class CollaboratorOutput
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }

    public CollaboratorOutput()
    {
    }

    public CollaboratorOutput(Collaborator collaborator)
    {
        Id = collaborator.Id;
        UserId = collaborator.UserId;
        ProjectId = collaborator.ProjectId;
    }
}