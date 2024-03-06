using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TokenRefresh.Api.Abstractions;
using TokenRefresh.Api.Data;
using TokenRefresh.Api.Models;

namespace TokenRefresh.Api.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions;
        private readonly ApplicationDbContext _context;

        public JwtProvider(IOptions<JwtOptions> jwtOptions, ApplicationDbContext context)
        {
            _jwtOptions = jwtOptions.Value;
            _context = context;
        }

        public async Task<AuthResult> GenerateTokenAsync(IdentityUser user)
        {
            JsonWebTokenHandler handler = new();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            var validKey = new SymmetricSecurityKey(key);
            var jtd = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.Add(_jwtOptions.AccessTokenExpires),
                IssuedAt = DateTime.UtcNow,
                TokenType = "at+jwt",
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
                    new(JwtRegisteredClaimNames.Sub, user.Email.ToString()),
                    new(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.ToString()),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString())

                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var jws = handler.CreateToken(jtd);

            var validationResult = handler.ValidateTokenAsync(jws, new TokenValidationParameters
            {
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = validKey

            });
            Console.WriteLine(validationResult.IsCompletedSuccessfully);

            //RefreshToken
            var refreshToken = new RefreshToken()
            {
                Jti = jws,
                Token = GenerateKeyToRefreshToken(),
                AddedDate = DateTime.UtcNow,
                ExpireDate = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                IsUsed = false,
                UserId = user.Id

            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            _ = _context.SaveChangesAsync().ConfigureAwait(false);

            return new AuthResult()
            {
                Jwtoken = jws,
                RefreshToken = refreshToken.Token,
                Result = true
            };

        }

        private static string GenerateKeyToRefreshToken()
        {
            var data = Guid.NewGuid().ToString("N");
            return data;
        }

    }
}
