using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Coders_Back.Infrastructure.DataAbstractions;

public class Repository <TEntity> : IRepository <TEntity> where TEntity: class {  
    
    private readonly AppDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public DbSet<TEntity> GetDbSet() => _dbSet;
    
    public IEnumerable<TEntity> GetAll() => _dbSet;
    public async Task<TEntity?> GetById(Guid id) => await _dbSet.FindAsync(id);
    
    public async Task Insert(TEntity obj) => await _dbSet.AddAsync(obj);
    
    public void Update(TEntity obj)
    {
        _dbSet.Attach(obj);
        _context.Entry(obj).State = EntityState.Modified;
    }

    public async Task Delete(Guid id)
    {
        var entityToDelete = await GetById(id);
        if (entityToDelete is not null) Delete(entityToDelete);
    }
    
    public void Delete(TEntity obj) => _dbSet.Remove(obj);
    
}  