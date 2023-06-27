using Coders_Back.Infrastructure.EntityFramework.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coders_Back.Infrastructure.EntityFramework;

public static class DatabaseManagementService
{
    public static async Task InitializeDatabaseMigrations(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILogger<AppDbContext>>();
        try
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!await dbContext.Database.CanConnectAsync())
            {
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("DB migrations successfully applied");
            }
        }
        catch (SqlException e)
        {
            logger.LogError("There was an error trying to apply DB migrations: {E}. Try not to use the --force-recreate command to bring up the container",
                e);
            throw;
        }
        catch (Exception e)
        {
            logger.LogError("There was an error trying to apply DB migrations: {E}", e);
            throw;
        }

    }
}