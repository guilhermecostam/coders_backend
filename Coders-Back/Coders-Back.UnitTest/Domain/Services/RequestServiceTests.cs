using Coders_Back.Domain.Enums;
using Coders_Back.Domain.Services;
using Coders_Back.UnitTest.Domain.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Coders_Back.UnitTest.Domain.Services;

public class RequestServiceTests
{
    [Fact(DisplayName = "Try to create a valid request")]
    public async Task TryCreateValidRequest()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, null!);
        
        var result = await service.Create(utils.Projects[0].Id, utils.Users[0].Id);
        
        var requestDbSet = utils.RequestsRepo.GetDbSet();
        var request = await requestDbSet.FirstAsync(r => r.UserId == utils.Users[0].Id && r.ProjectId == utils.Projects[0].Id);
        
        request.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Error.Should().BeNull();
    }
    
    [Fact(DisplayName = "Try to create a request when project not exist")]
    public async Task TryCreateRequestWithInvalidProject()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, null!);
        var requestsCount = (await utils.RequestsRepo.GetAll()).Count;
        
        var result = await service.Create(Guid.NewGuid(), utils.Users[0].Id);
        
        var requests = await utils.RequestsRepo.GetAll();
        requests.Count.Should().Be(requestsCount);

        result.Success.Should().BeFalse();
        result.Error.Should().Be(RequestCreateOutputError.ProjectNotFound);
    }
    
    [Fact(DisplayName = "Try to create a request that already exists")]
    public async Task TryCreateRequestAlreadyExists()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, null!);

        var requestsCount = (await utils.RequestsRepo.GetAll()).Count;
        
        var result = await service.Create(utils.Requests[1].ProjectId, utils.Requests[1].UserId);
        
        var requests = await utils.RequestsRepo.GetAll();
        requests.Count.Should().Be(requestsCount);

        result.Success.Should().BeFalse();
        result.Error.Should().Be(RequestCreateOutputError.RequestAlreadyExists);
    }
    
    [Fact(DisplayName = "Try to create a request that userId already is a collaborator")]
    public async Task TryCreateRequestUserAlreadyIsCollaborator()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, null!);

        var requestsCount = (await utils.RequestsRepo.GetAll()).Count;
        var result = await service.Create(utils.Collaborators[0].ProjectId, utils.Collaborators[0].UserId);
        
        var requests = await utils.RequestsRepo.GetAll();
        requests.Count.Should().Be(requestsCount);

        result.Success.Should().BeFalse();
        result.Error.Should().Be(RequestCreateOutputError.CollaboratorAlreadyExists);
    }
    
    [Fact(DisplayName = "Try to get all requests made for a user")]
    public async Task TryGetRequestsByUser()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, null!);

        var result = await service.GetByUser(utils.Requests[0].UserId);

        result.Count.Should().Be(1);
        result[0].UserId.Should().Be(utils.Requests[0].UserId);
    }
    
    [Fact(DisplayName = "Try to get pending requests by user")]
    public async Task TryGetPendingRequestsByUser()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, null!);

        var result = await service.GetPendingByUser(utils.Users[2].Id);

        result.Count.Should().Be(2);
    }
    
    [Fact(DisplayName = "Try to get pending requests by project")]
    public async Task TryGetPendingRequestsByProject()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, null!);

        var result = await service.GetPendingByProject(utils.Projects[2].Id);

        result.Count.Should().Be(2);
        foreach (var project in result)
        {
            project.ProjectId.Should().Be(utils.Projects[2].Id);
        }
    }
    
    [Fact(DisplayName = "Try to reject pending request")]
    public async Task TryRejectRequest()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.ProjectServiceMock.Object);

        var result = await service.Reject(utils.Requests[3].Id, Guid.NewGuid());

        result.Should().BeTrue();
        var request = await utils.RequestsRepo.GetById(utils.Requests[3].Id);
        request.Should().NotBeNull();
        request!.Status.Should().Be(RequestStatus.Rejected);
    }
    
    [Fact(DisplayName = "Try to reject a invalid request")]
    public async Task TryRejectInvalidRequest()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.ProjectServiceMock.Object);

        var result = await service.Reject(Guid.NewGuid(), Guid.NewGuid());

        result.Should().BeFalse();
    }
    
    [Fact(DisplayName = "Try to accept pending request")]
    public async Task TryAcceptRequest()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.ProjectServiceMock.Object);

        var result = await service.Accept(utils.Requests[3].Id, Guid.NewGuid());

        result.Should().BeTrue();
        var request = await utils.RequestsRepo.GetById(utils.Requests[3].Id);
        request.Should().NotBeNull();
        request!.Status.Should().Be(RequestStatus.Accepted);
        
        var collaboratorDbSet = utils.CollaboratorsRepo.GetDbSet();
        var collaborator = await collaboratorDbSet.FirstAsync(c =>
            c.UserId == utils.Requests[3].UserId && c.ProjectId == utils.Requests[3].ProjectId);
        collaborator.Should().NotBeNull();
    }
    
    [Fact(DisplayName = "Try to accept a invalid pending request")]
    public async Task TryAcceptInvalidRequest()
    {
        var utils = await RequestServiceTestsUtils.NewUtils();
        var service = new RequestService(utils.ProjectsRepo, utils.RequestsRepo, utils.UnitOfWork, utils.CollaboratorsRepo, utils.ProjectServiceMock.Object);

        var result = await service.Accept(Guid.NewGuid(), Guid.NewGuid());

        result.Should().BeFalse();
    }
}