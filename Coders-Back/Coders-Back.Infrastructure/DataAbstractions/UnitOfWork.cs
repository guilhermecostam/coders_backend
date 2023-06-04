using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Infrastructure.EntityFramework.Context;

namespace Coders_Back.Infrastructure.DataAbstractions;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    
    public void Dispose() => _context.Dispose();

    public async ValueTask DisposeAsync() => await _context.DisposeAsync();
}