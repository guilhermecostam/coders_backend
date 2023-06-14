namespace Coders_Back.Domain.Interfaces;

public interface IGithubApi
{
    Task<List<string>?> GetTechnologiesByProject(string? ghOwner, string? ghRepoUrl);
}