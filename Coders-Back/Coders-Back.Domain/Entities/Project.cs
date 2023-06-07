using Coders_Back.Domain.DataAbstractions;

namespace Coders_Back.Domain.Entities;

public class Project : IEntity, ISoftDelete
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? GithubUrl { get; set; }
    public Guid OwnerId { get; set; }
    public string? DiscordUrl { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; }
}
