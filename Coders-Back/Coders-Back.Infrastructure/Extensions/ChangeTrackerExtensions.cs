using Coders_Back.Domain.DataAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Coders_Back.Infrastructure.Extensions;

public static class ChangeTrackerExtensions
{
    public static void ApplySoftDelete(this ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();
        var entities =
            changeTracker
                .Entries()
                .Where(t => t.Entity is ISoftDelete && t.State == EntityState.Deleted);

        var entityEntries = entities.ToList();
        if (!entityEntries.Any()) return;
        
        foreach (var entry in entityEntries)
        {
            var entity = (ISoftDelete)entry.Entity;
            entity.IsDeleted = true;
            entry.State = EntityState.Modified;
        }
    }
}  