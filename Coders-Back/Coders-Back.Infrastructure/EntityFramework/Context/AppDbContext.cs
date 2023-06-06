using Coders_Back.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coders_Back.Infrastructure.EntityFramework.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<Project> Projects { get; set;}
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}