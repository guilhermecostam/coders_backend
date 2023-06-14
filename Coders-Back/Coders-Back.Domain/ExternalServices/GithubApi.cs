using System.Net.Http.Headers;
using System.Text.Json;
using Coders_Back.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Coders_Back.Domain.ExternalServices;

public class GithubApi : IGithubApi
{
    private readonly IMemoryCache _memoryCache;

    public GithubApi(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<List<string>?> GetTechnologiesByProject(string? ghOwner, string? ghRepoUrl)
    {
        if (string.IsNullOrEmpty(ghRepoUrl) || string.IsNullOrEmpty(ghOwner)) return null;
        if (_memoryCache.TryGetValue($"{ghOwner}-{ghRepoUrl}", out string? languagesList)) 
            return GetLanguagesByJson(languagesList);
        
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("coders", "0"));
        var apiUrl = $"https://api.github.com/repos/{ghOwner}/{ghRepoUrl}/languages";

        var response = await httpClient.GetAsync(apiUrl);
        if (!response.IsSuccessStatusCode) return null;
        var responseContent = await response.Content.ReadAsStringAsync();
        _memoryCache.Set($"{ghOwner}-{ghRepoUrl}", responseContent, TimeSpan.FromDays(5));
        
        return GetLanguagesByJson(responseContent);
    }

    private static List<string>? GetLanguagesByJson(string? json)
    {
        var languages = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
        return languages is null ? null : new List<string>(languages.Keys);
    }
}