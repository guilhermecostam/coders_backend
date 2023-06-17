using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Interfaces;
using Moq;

namespace Coders_Back.UnitTest.Domain.Utils;

public class ProjectServiceTestsUtils : UnitTestBaseUtils
{
    public IRepository<Project> ProjectsRepo { get; set; }
    public IRepository<ApplicationUser> UsersRepo { get; set; }
    public IRepository<Collaborator> CollaboratorsRepo { get; set; }
    public IUnitOfWork UnitOfWork { get; set; }
    public Mock<IGithubApi> GithubApiMock { get; set; }
    public List<Project> Projects { get; set; }
    public List<ApplicationUser> Users { get; set; }
    public List<Collaborator> Collaborators { get; set; }

    public static async Task<ProjectServiceTestsUtils> NewUtils()
    {
        var utils = new ProjectServiceTestsUtils();
        
        var usersDbSet = utils.UsersRepo.GetDbSet();
        await usersDbSet.AddRangeAsync(utils.Users);
        
        var projectsDbSet = utils.ProjectsRepo.GetDbSet();
        await projectsDbSet.AddRangeAsync(utils.Projects);
        
        var collaboratorsDbSet = utils.CollaboratorsRepo.GetDbSet();
        await collaboratorsDbSet.AddRangeAsync(utils.Collaborators);
        
        await utils.UnitOfWork.SaveChangesAsync();
        
        return utils;
    }

    private ProjectServiceTestsUtils()
    {
        UnitOfWork = GetInMemoryUnitOfWork();
        ProjectsRepo = GetInMemoryRepository<Project>();
        UsersRepo = GetInMemoryRepository<ApplicationUser>();
        CollaboratorsRepo = GetInMemoryRepository<Collaborator>();
        
        GithubApiMock = new Mock<IGithubApi>();
        GithubApiMock.Setup(m => m.GetTechnologiesByProject(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<string>
        {
            "Cobol",
            "C",
            "Fortran",
            "Portugol"
        });

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
                OwnerId = Users[0].Id,
                DiscordUrl = null,
                DateCreation = default,
                IsDeleted = false,
                Technologies = null
            },
            new Project
            {
                Id = Guid.NewGuid(),
                Name = "SomeProject",
                OwnerId = Users[1].Id
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
            },
            new Collaborator
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ProjectId = Projects[2].Id
            }
        };
    }
}