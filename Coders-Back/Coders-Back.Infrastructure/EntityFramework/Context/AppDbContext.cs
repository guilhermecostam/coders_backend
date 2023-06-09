using System.Linq.Expressions;
using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.Entities;
using Coders_Back.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coders_Back.Infrastructure.EntityFramework.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<Project> Projects { get; set;}
    public DbSet<Collaborator> Collaborators { get; set;}
    public DbSet<ProjectJoinRequest> ProjectJoinRequests { get; set;}
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    #region overrides

        protected override void OnModelCreating(ModelBuilder builder)
        {
            AddIsDeletedQueryFilter(builder);

            builder.Entity<Project>().Property(p => p.OwnerId).IsRequired();
            builder.Entity<Project>().Property(p => p.Name).IsRequired();
            
            builder.Entity<Collaborator>().Property(c => c.UserId).IsRequired();
            builder.Entity<Collaborator>().Property(c => c.ProjectId).IsRequired();
            builder.Entity<Collaborator>()
                .HasIndex(c => new { c.UserId, c.ProjectId })
                .IsUnique();

            builder.Entity<ProjectJoinRequest>().Property(r => r.ProjectId).IsRequired();
            builder.Entity<ProjectJoinRequest>().Property(r => r.UserId).IsRequired();
            builder.Entity<ProjectJoinRequest>()
                .HasIndex(r => new { r.UserId, r.ProjectId })
                .IsUnique();

            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.ApplySoftDelete();
            return base.SaveChanges();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTracker.ApplySoftDelete();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.ApplySoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.ApplySoftDelete();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

    #endregion
    
    private static LambdaExpression SoftDeleteQueryFilterLambda(Type type)
    {
        var parameter = Expression.Parameter(type, "e");
        var falseConstantValue = Expression.Constant(false);
        var propertyAccess = Expression.PropertyOrField(parameter, nameof(ISoftDelete.IsDeleted));
        var equalExpression = Expression.Equal(propertyAccess, falseConstantValue);
        return Expression.Lambda(equalExpression, parameter);
    }

    private static void AddIsDeletedQueryFilter(ModelBuilder builder)
    {
        var softDeleteEntities = typeof(ISoftDelete).Assembly.GetTypes()
            .Where(type => typeof(ISoftDelete)
                               .IsAssignableFrom(type)
                           && type is { IsClass: true, IsAbstract: false });

        foreach (var softDeleteEntity in softDeleteEntities)
        {
            builder.Entity(softDeleteEntity).HasQueryFilter(SoftDeleteQueryFilterLambda(softDeleteEntity));
        }
    }
}