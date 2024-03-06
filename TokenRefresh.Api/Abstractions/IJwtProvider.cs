using Microsoft.AspNetCore.Identity;
using TokenRefresh.Api.Models;

namespace TokenRefresh.Api.Abstractions
{
    public interface IJwtProvider
    {
        Task<AuthResult> GenerateTokenAsync(IdentityUser user);
    }
}
