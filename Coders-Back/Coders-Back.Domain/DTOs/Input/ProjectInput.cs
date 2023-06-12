using System.ComponentModel.DataAnnotations;

namespace Coders_Back.Domain.DTOs.Input;

public class ProjectInput
{
    public ProjectInput(string name, string description, string githubUrl, string discordUrl)
    {
        Name = name;
        Description = description;
        GithubUrl = githubUrl;
        DiscordUrl = discordUrl;
    }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    public string Description { get; set; }
    
    //TODO: block to be a valid url
    public string GithubUrl { get; set; }
    
    //TODO: block to be a valid url
    public string DiscordUrl { get; set; }
}