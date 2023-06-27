using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Enums;
using Coders_Back.Domain.Interfaces;
using Moq;

namespace Coders_Back.UnitTest.Domain.Utils;

public class RequestServiceTestsUtils : UnitTestBaseUtils
{
    public IRepository<ProjectJoinRequest> RequestsRepo { get; set; }
    public IRepository<Project> ProjectsRepo { get; set; }
    public IRepository<ApplicationUser> UsersRepo { get; set; }
    public IRepository<Collaborator> CollaboratorsRepo { get; set; }
    public IUnitOfWork UnitOfWork { get; set; }
    public Mock<IProjectService> ProjectServiceMock { get; set; }
    public List<Project> Projects { get; set; }
    public List<ApplicationUser> Users { get; set; }
    public List<Collaborator> Collaborators { get; set; }
    public List<ProjectJoinRequest> Requests { get; set; }

    public static async Task<RequestServiceTestsUtils> NewUtils()
    {
        var utils = new RequestServiceTestsUtils();
        
        var usersDbSet = utils.UsersRepo.GetDbSet();
        await usersDbSet.AddRangeAsync(utils.Users);
        
        var projectsDbSet = utils.ProjectsRepo.GetDbSet();
        await projectsDbSet.AddRangeAsync(utils.Projects);
        
        var collaboratorsDbSet = utils.CollaboratorsRepo.GetDbSet();
        await collaboratorsDbSet.AddRangeAsync(utils.Collaborators);
        
        var requestsDbSet = utils.RequestsRepo.GetDbSet();
        await requestsDbSet.AddRangeAsync(utils.Requests);
        
        await utils.UnitOfWork.SaveChangesAsync();
        
        return utils;
    }

    private RequestServiceTestsUtils()
    {
        UnitOfWork = GetInMemoryUnitOfWork();
        RequestsRepo = GetInMemoryRepository<ProjectJoinRequest>();
        ProjectsRepo = GetInMemoryRepository<Project>();
        UsersRepo = GetInMemoryRepository<ApplicationUser>();
        CollaboratorsRepo = GetInMemoryRepository<Collaborator>();
        ProjectServiceMock = new Mock<IProjectService>();
        ProjectServiceMock.Setup(m => 
            m.IsProjectOwner(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

        Users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                UserName = "ASmartestTester",
                Email = "smartersttester@email.com",
                Id = Guid.NewGuid(),
                Name = "A Smartest Tester",
                BirthDate = DateTime.Now.AddYears(-20),
                GithubProfile = "tester",
                AddressId = default,
                LinkedinUrl = null
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Name = "name"
            }, 
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Name = "The User That Will Change Your Life"
            }
        };
        
        Projects = new List<Project>
        {
            new Project
            {
                Id = Guid.NewGuid(),
                Name = "Coders_Backend",
                Description = "A amazing project constructed for Software Engineering II",
                GithubUrl = "https://github.com/guilhermecostam/coders_backend",
                OwnerId = default,
                DiscordUrl = null,
                DateCreation = default,
                IsDeleted = false,
                Technologies = null
            },
            new Project
            {
                Id = Guid.NewGuid(),
                Name = "SomeProject"
            },
            new Project
            {
                Id = Guid.NewGuid(),
                Name = "A project that will change your life",
                OwnerId = Users[2].Id
            }
        };
        
        Collaborators = new List<Collaborator>
        {
            new Collaborator
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ProjectId = Projects[1].Id
            },
            new Collaborator
            {
                Id = Guid.NewGuid(),
                UserId = Users[2].Id,
                ProjectId = Projects[2].Id
            }
        };

        Requests = new List<ProjectJoinRequest>
        {
            new ProjectJoinRequest
            {
                Id = Guid.NewGuid(),
                Status = RequestStatus.Open,
                UserId = Guid.NewGuid(),
                ProjectId = default
            }, 
            new ProjectJoinRequest
            {
                Id = Guid.NewGuid(),
                Status = RequestStatus.Open,
                UserId = Users[1].Id,
                ProjectId = Projects[1].Id
            },
            new ProjectJoinRequest
            {
                Id = Guid.NewGuid(),
                Status = RequestStatus.Open,
                UserId = Guid.NewGuid(),
                ProjectId = Projects[2].Id
            },
            new ProjectJoinRequest
            {
                Id = Guid.NewGuid(),
                Status = RequestStatus.Open,
                UserId = Guid.NewGuid(),
                ProjectId = Projects[2].Id
            }
        };
    }
}