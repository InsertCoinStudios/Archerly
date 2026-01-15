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

    public static bool TryGetUserGuidFromClaim(string caller, ClaimsPrincipal user, [NotNullWhen(true)] out Guid userId, [NotNullWhen(false)] out IResult? error)
    {

        var _userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? user.FindFirst(Claims.Subject)?.Value;

        if (_userId == null)
        {
            error = Results.Problem(new ApiError(
                "claim_does_not_include_sub",
                "Sub Claim is missing from the claims of the JWT")
                .ToString(),
                type: $"{caller}:failed",
                title: "claim_does_not_include_sub",
                statusCode: 500
                );
            userId = Guid.Empty;
            return false;
        }

        var guid = Guid.Parse(_userId);
        if (Guid.Empty.Equals(guid))
        {
            error = Results.Problem(new ApiError(
                "sub_claim_guid_parse_failed",
                "The provided sub claim could not be parsed into a Guid")
                .ToString(),
                type: $"{caller}:failed",
                title: "sub_claim_guid_parse_failed",
                statusCode: 500
                );
            userId = Guid.Empty;
            return false;
        }
        userId = guid;
        error = null;
        return true;
    }

    public static bool TryGetUserGuidFromToken(string caller, JwtSecurityToken token, [NotNullWhen(true)] out Guid userId, [NotNullWhen(false)] out IResult? error)
    {
        var _userId = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (_userId == null)
        {
            error = Results.Problem(new ApiError(
                "claim_does_not_include_sub",
                "Sub Claim is missing from the claims of the JWT")
                .ToString(),
                type: $"{caller}:failed",
                title: "claim_does_not_include_sub",
                statusCode: 500
                );
            userId = Guid.Empty;
            return false;
        }

        var guid = Guid.Parse(_userId);
        if (Guid.Empty.Equals(guid))
        {
            error = Results.Problem(new ApiError(
                "sub_claim_guid_parse_failed",
                "The provided sub claim could not be parsed into a Guid")
                .ToString(),
                type: $"{caller}:failed",
                title: "sub_claim_guid_parse_failed",
                statusCode: 500
                );
            userId = Guid.Empty;
            return false;
        }
        userId = guid;
        error = null;
        return true;
    }


    public static bool TryGetUserGuidFromRawToken(string caller, string user, [NotNullWhen(true)] out Guid userId, [NotNullWhen(false)] out IResult? error)
    {
        var token = ParseJWT(user);
        return TryGetUserGuidFromToken(caller, token, out userId, out error);
    }

}