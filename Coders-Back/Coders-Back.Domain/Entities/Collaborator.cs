using Coders_Back.Domain.DataAbstractions;

namespace Coders_Back.Domain.Entities;

public class Collaborator : IEntity, ISoftDelete
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsDeleted { get; set; }
}