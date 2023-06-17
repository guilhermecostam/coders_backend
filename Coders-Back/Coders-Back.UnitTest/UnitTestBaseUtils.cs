using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Infrastructure.DataAbstractions;
using Coders_Back.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using Xunit.Sdk;

namespace Coders_Back.UnitTest
{
    public class UnitTestBaseUtils
    {
        private AppDbContext? Context { get; }

        protected UnitTestBaseUtils()
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseInMemoryDatabase("AppDbContextInMemory");
            var options = builder.Options;
            Context = new AppDbContext(options);
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }

        protected IRepository<TEntity> GetInMemoryRepository<TEntity>() where TEntity : class
        {
            if (Context is not null) return new Repository<TEntity>(Context);
            throw new NullException(typeof(AppDbContext));
        }
        
        protected IUnitOfWork GetInMemoryUnitOfWork()
        {
            if (Context is not null) return new UnitOfWork(Context);
            throw new NullException(typeof(AppDbContext));
        }
        
        public void Dispose()
        {
            Context?.Database.EnsureDeleted();
        }
    }
}