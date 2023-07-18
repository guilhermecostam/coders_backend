using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Services;
using Coders_Back.UnitTest.Utils;
using FluentAssertions;
using Moq;
using Xunit;

namespace Coders_Back.UnitTest.Domain.Services;

public class ProjectServiceTests
{
    [Fact(DisplayName = "Try to get all projects")]
    public async void TryGetAllProjects()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();

        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, null, null, utils.GithubApiMock.Object);
        
        var result = await service.GetAll();

        result.Count.Should().Be(3);
        utils.GithubApiMock.Verify(m => m.GetTechnologiesByProject(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(result.Count));
    }
    
    [Fact(DisplayName = "Try to get project by id")]
    public async void TryGetProjectById()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();

        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, null, null, utils.GithubApiMock.Object);
        
        var result = await service.GetById(utils.Projects[0].Id);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo( new ProjectOutput(utils.Projects[0]) );
        utils.GithubApiMock.Verify(m => m.GetTechnologiesByProject(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact(DisplayName = "Try to create a new project")]
    public async void TryCreateProject()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();

        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.GithubApiMock.Object);
        
        var newProject = new ProjectInput("NewProject", "Created On Unit Tests",
            "ImagineAUrlHere", "ImagineAUrlHereToo");

        var collaboratorCount = (await utils.CollaboratorsRepo.GetAll()).Count;
        
        var result = await service.CreateProject(newProject, utils.Users[0].Id);
        
        var projectResult = await utils.ProjectsRepo.GetById(result.Id);
        var collaborators = await utils.CollaboratorsRepo.GetAll();
        
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new ProjectOutput(projectResult!));
        collaborators.Count.Should().Be(collaboratorCount+1);
        collaborators[^1].UserId.Should().Be(utils.Users[0].Id);
    }
    
    [Fact(DisplayName = "Try to update a project with invalid projectId")]
    public async void TryUpdateInvalidProject()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();
        
        var newProject = new ProjectInput("NewProject", "Created On Unit Tests",
            "ImagineAUrlHere", "ImagineAUrlHereToo");

        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.GithubApiMock.Object);

        var result = await service.UpdateProject(Guid.NewGuid(), newProject);

        result.Should().BeFalse();
    }
    
    [Fact(DisplayName = "Try to update a project")]
    public async void TryUpdateProject()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();
        
        var newProject = new ProjectInput("NewProject", "Created On Unit Tests",
            "ImagineAUrlHere", "ImagineAUrlHereToo");

        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.GithubApiMock.Object);

        var result = await service.UpdateProject(utils.Projects[2].Id, newProject);

        result.Should().BeTrue();
        var project = await utils.ProjectsRepo.GetById(utils.Projects[2].Id);
        project!.Name.Should().Be(newProject.Name);
        project.Description.Should().Be(newProject.Description);
        project.GithubUrl.Should().Be(newProject.GithubUrl);
        project.DiscordUrl.Should().Be(newProject.DiscordUrl);
    }
    
    [Fact(DisplayName = "Try to delete a project with invalid projectId")]
    public async void TryDeleteInvalidProject()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();
        
        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.GithubApiMock.Object);

        var result = await service.DeleteProject(Guid.NewGuid());

        result.Should().BeFalse();
    }
    
    [Fact(DisplayName = "Try to delete a project")]
    public async void TryDeleteProject()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();
        var projectsCount = (await utils.ProjectsRepo.GetAll()).Count;
        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.GithubApiMock.Object);
        
        var result = await service.DeleteProject(utils.Projects[0].Id);
        
        var projectsCountAfterAction = (await utils.ProjectsRepo.GetAll()).Count;
        var project = await utils.ProjectsRepo.GetById(utils.Projects[0].Id);
        
        result.Should().BeTrue();
        projectsCountAfterAction.Should().Be(projectsCount - 1);
        project?.IsDeleted.Should().BeTrue();
    }
    
    [Fact(DisplayName = "Try to get all collaborators by project with invalid projectId")]
    public async void TryGetCollaboratorsWithInvalidProjectId()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();
        
        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.GithubApiMock.Object);
        
        var result = await service.GetCollaboratorsByProject(Guid.NewGuid());

        result.Should().BeNull();
    }
    
    [Fact(DisplayName = "Try to get all collaborators by project")]
    public async void TryGetCollaboratorsByProject()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();
        
        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.GithubApiMock.Object);
        
        var result = await service.GetCollaboratorsByProject(utils.Projects[2].Id);

        result.Should().NotBeNull();
        result!.Count.Should().Be(2);
        foreach (var collaboratorOutput in result)
        {
            collaboratorOutput.ProjectId.Should().Be(utils.Projects[2].Id);
        }
    }
    
    [Fact(DisplayName = "Try to delete a collaborator")]
    public async void TryDeleteCollaborator()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();
        var collaboratorsCount = (await utils.CollaboratorsRepo.GetAll()).Count;
        
        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.GithubApiMock.Object);
        
        await service.DeleteCollaborator(utils.Collaborators[1].Id, utils.Collaborators[1].UserId);

        var collaboratorsCountAfterAction = (await utils.CollaboratorsRepo.GetAll()).Count;
        var collaborator = await utils.CollaboratorsRepo.GetById(utils.Collaborators[1].Id);
        
        collaboratorsCountAfterAction.Should().Be(collaboratorsCount - 1);
        collaborator?.IsDeleted.Should().BeTrue();
    }
    
    [Fact(DisplayName = "Verify IsProjectOwner when should be false")]
    public async void VerifyIsProjectOwnerWhenFalse()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();

        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.GithubApiMock.Object);

        var result = await service.IsProjectOwner(Guid.NewGuid(), Guid.NewGuid());

        result.Should().BeFalse();
    }
    
    [Fact(DisplayName = "Verify IsProjectOwner when should be true")]
    public async void VerifyIsProjectOwnerWhenTrue()
    {
        var utils = await ProjectServiceTestsUtils.NewUtils();

        var service = new ProjectService(utils.ProjectsRepo, utils.UsersRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.GithubApiMock.Object);

        var result = await service.IsProjectOwner(utils.Users[2].Id, utils.Projects[2].Id);

        result.Should().BeTrue();
    }
}
