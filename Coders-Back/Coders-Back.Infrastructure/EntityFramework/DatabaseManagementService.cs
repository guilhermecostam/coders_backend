using Coders_Back.Infrastructure.EntityFramework.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coders_Back.Infrastructure.EntityFramework;

public static class DatabaseManagementService
{
    public static void InitializeDatabaseMigrations(this IApplicationBuilder app)
    {
        try
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            serviceScope.ServiceProvider.GetService<AppDbContext>()?.Database.Migrate();
        }
        catch (Exception e)
        {
            var serviceScope = app.ApplicationServices.GetRequiredService<ILogger<AppDbContext>>();
            serviceScope.LogError("There was an error trying to apply DB migrations: {E}", e);
            throw;
        }

    }
}