using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Coders_Back.Domain.Services;

public class UserService : IUserService
{
    //TODO: implements address
    private readonly IRepository<ApplicationUser> _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;
    private readonly IRepository<Project> _projects;

    public UserService(IRepository<ApplicationUser> users, IUnitOfWork unitOfWork, ILogger<UserService> logger, IRepository<Project> projects)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _projects = projects;
    }

    public async Task<bool> UpdateUser(UserInput input)
    {
        var user = await _users.GetById(input.Id);
        if(user is null) return false;
        
        if (input.BirthDate.Year > DateTime.Now.Year) 
            return false;

        user.BirthDate = input.BirthDate;
        user.Name = input.Name;
        user.GithubProfile = input.GithubProfile;
        user.LinkedinUrl = input.LinkedinUrl;
        user.PhoneNumber = input.PhoneNumber;
        //user.AddressId = user.AddressId;
        try
        {
            _users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("There was a error try updating User", e);
            return false;
        }
        return true;
    }

    public async Task<List<ProjectOutput>?> GetProjectsByUser(Guid id)
    {
        var user = await _users.GetById(id);
        if (user is null) return null;

        var projects = await _projects.GetAll();
        return projects.Where(p => p.OwnerId == id).Select(p => new ProjectOutput(p)).ToList();
    }
}