using Coders_Back.Domain.Entities;

namespace Coders_Back.Domain.DTOs.Output;

public class ProjectOutput
{
    public ProjectOutput()
    {
    }
    
    public ProjectOutput(Project input)
    {
        Id = input.Id;
        Name = input.Name;
        Description = input.Description;
        GithubUrl = input.GithubUrl;
        OwnerId = input.OwnerId;
        DiscordUrl = input.DiscordUrl;
        Technologies = input.Technologies;
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? GithubUrl { get; set; }
    public Guid OwnerId { get; set; }
    public string? DiscordUrl { get; set; }
    
    public List<string>? Technologies { get; set; }
}