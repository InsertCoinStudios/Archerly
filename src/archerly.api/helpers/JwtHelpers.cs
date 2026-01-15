using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using archerly.api.endpoints;
using archerly.core;

namespace archerly.api.helpers;

public static class JwtHelpers
{
    public static bool TryGetUserId(ClaimsPrincipal user, [NotNullWhen(true)] out string? userId)
    {
        var _userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? user.FindFirst(Claims.Subject)?.Value;

        if (_userId == null)
        {
            userId = null;
            return false;
        }
        userId = _userId;
        return true;
    }


    public static bool TryGetUserId(JwtSecurityToken token, [NotNullWhen(true)] out string? userId)
    {
        var subClaim = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (subClaim == null)
        {
            userId = string.Empty;
            return false;
        }
        userId = subClaim;
        return true;
    }

    public static bool TryGetUserGuid(JwtSecurityToken token, [NotNullWhen(true)] out Guid userId)
    {
        var subClaim = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (subClaim == null)
        {
            userId = Guid.Empty;
            return false;
        }
        userId = Guid.Parse(subClaim);
        return true;
    }

    public static JwtSecurityToken ParseJWT(string? jwt)
    {
        var handler = new JwtSecurityTokenHandler();

        return handler.ReadJwtToken(jwt);
    }


}