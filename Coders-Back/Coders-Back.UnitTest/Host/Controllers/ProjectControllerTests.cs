using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Interfaces;
using Coders_Back.Host.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Coders_Back.UnitTest.Host.Controllers;

public class ProjectControllerTests : UnitTestBaseUtils
{
    private readonly Mock<IProjectService> _projectServiceMock;
    private readonly Mock<IRequestService> _requestServiceMock;
    private readonly ProjectController _controller;

    public ProjectControllerTests()
    {
        _projectServiceMock = new Mock<IProjectService>();
        _requestServiceMock = new Mock<IRequestService>();
        _controller = new ProjectController(_projectServiceMock.Object, _requestServiceMock.Object);
    }

    [Fact(DisplayName = "Get all projects should return OK result")]
    public async Task GetAll_ShouldReturnOkResult()
    {
        var projects = new List<ProjectOutput> { /* Preencha com projetos de teste */ };
        _projectServiceMock.Setup(s => s.GetAll()).ReturnsAsync(projects);

        var result = await _controller.GetAll();

        result.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeEquivalentTo(projects);
    }

    [Fact(DisplayName = "Get project by valid id should return OK result")]
    public async Task GetByIdAsync_WithValidId_ShouldReturnOkResult()
    {
        var projectId = Guid.NewGuid();
        var project = new ProjectOutput { /* Preencha com um projeto de teste */ };
        _projectServiceMock.Setup(s => s.GetById(projectId)).ReturnsAsync(project);

        var result = await _controller.GetByIdAsync(projectId);

        result.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeEquivalentTo(project);
    }

    [Fact(DisplayName = "Get project by invalid id should return NotFound result")]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNotFoundResult()
    {
        var projectId = Guid.NewGuid();
        _projectServiceMock.Setup(s => s.GetById(projectId)).ReturnsAsync((ProjectOutput)null);

        var result = await _controller.GetByIdAsync(projectId);

        result.Should().BeOfType<NotFoundResult>();
    }


    [Fact(DisplayName = "Delete collaborator with valid ids should return NoContent result", Skip = "fix this test")]
    public async Task DeleteCollaborator_WithValidIds_ShouldReturnNoContentResult()
    {
        var projectId = Guid.NewGuid();
        var collaboratorId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var isOwner = true;

        _projectServiceMock.Setup(s => s.IsProjectOwner(userId, projectId)).ReturnsAsync(isOwner);
        
        var result = await _controller.DeleteCollaborator(projectId, collaboratorId);
        
        result.Should().BeOfType<NoContentResult>();
        _projectServiceMock.Verify(s => s.DeleteCollaborator(collaboratorId, userId), Times.Once);
    }
}
