using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TokenRefresh.Api.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string? GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(JwtRegisteredClaimNames.GivenName);
        }

        public static string? GetId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(JwtRegisteredClaimNames.NameId.ToString());
        }
    }
}
