using Microsoft.EntityFrameworkCore;

namespace Coders_Back.Domain.DataAbstractions;

public interface IRepository<TEntity> where TEntity : class
{
    DbSet<TEntity> GetDbSet();
    Task<List<TEntity>> GetAll();
    Task<TEntity?> GetById(Guid id);
    Task Insert(TEntity obj);
    void Update(TEntity obj);
    Task Delete(Guid id);
    void Delete(TEntity obj);
}