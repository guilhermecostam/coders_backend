using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.Enums;

namespace Coders_Back.Domain.Entities;

public class ProjectJoinRequest : IEntity, ISoftDelete
{
    public Guid Id { get; set; }
    public RequestStatus Status { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; }
}