namespace Coders_Back.Domain.DataAbstractions;

public interface IUnitOfWork : IAsyncDisposable, IDisposable  
{
    Task SaveChangesAsync();
}