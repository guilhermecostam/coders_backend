using Coders_Back.Domain.Entities.Interfaces;

namespace Coders_Back.Domain.Entities;

public class Project : IEntity
{

    public string Name { get; set; }
    public Guid OwnerId { get; set; }

    public Guid Id { get; set; }
}