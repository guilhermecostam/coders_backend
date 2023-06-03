using Coders_Back.Domain.Entities;
using Coders_Back.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Coders_Back.Host.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetByUser(Guid id)
    {
        return await _context.Projects.Where(p => p.OwnerId == id).ToListAsync();
    }
}