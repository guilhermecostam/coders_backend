using System.Security.Claims;

namespace Coders_Back.Domain.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal claims)
    {
        var subClaim = claims.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
        if (subClaim is null) return null;
        Guid.TryParse(subClaim.Value, out var userId);
        return userId;
    }
}