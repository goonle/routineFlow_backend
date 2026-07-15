using System.Security.Claims;

namespace RoutineFlow.Middleware;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("No user id claim present.");
        return Guid.Parse(value);
    }
}
