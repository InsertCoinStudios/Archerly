using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using archerly.api.endpoints;

namespace archerly.api.helpers;

public static class JwtHelpers
{
    public static bool GetUserId(ClaimsPrincipal user, [NotNullWhen(true)] out string? userId)
    {
        var _userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? user.FindFirst(Claims.Subject)?.Value;

        if (_userId == null)
        {
            userId = null;
            return false;
        }
        else
        {
            userId = _userId;
            return true;
        }
    }
}