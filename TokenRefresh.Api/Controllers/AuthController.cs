using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TokenRefresh.Api.Abstractions;
using TokenRefresh.Api.Authentication;
using TokenRefresh.Api.Data;
using TokenRefresh.Api.InputModels;
using TokenRefresh.Api.Models;

namespace TokenRefresh.Api.Controllers
{
    [Route("")]
    [ApiController]
    public class AuthController(IJwtProvider jwtProvider,
                          UserManager<IdentityUser> userManager,
                          SignInManager<IdentityUser> signInManager,
                          ApplicationDbContext context,
                          IOptions<TokenValidationParameters> validationParameters,
                          IOptions<JwtOptions> jwtOptions) : ControllerBase
    {
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly ApplicationDbContext _context = context;
        private readonly TokenValidationParameters _validationParameters = validationParameters.Value;
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        [HttpPost("api/new-account")]
        public async Task<IActionResult> CreateUser(NewUser newUser)
        {
            var exists = await _userManager.FindByEmailAsync($"{newUser.Email}");
            if (exists is not null)
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors =
                    [
                        "Email alredy exist"
                    ]
                });

            IdentityUser user = new()
            {
                Email = newUser.Email,
                UserName = newUser.Email,
                EmailConfirmed = true,
                ConcurrencyStamp = DateTime.UtcNow.ToString()
            };
            var result = await _userManager.CreateAsync(user, newUser.Password);
            if (result.Succeeded)
            {
                return Ok(CreatedAtAction(nameof(CreateUser), new AuthResult()
                {
                    Result = true

                }));
            }
            return BadRequest(new AuthResult()
            {
                Errors =
                [
                    "Internal server error"
                ],
                Result = false

            });
        }

        [HttpPost("api/login")]
        public async Task<IActionResult> Login(UserLogin login)
        {
            var existing = await _userManager.FindByEmailAsync(login.Email);
            if (existing is null)
            {
                return BadRequest(new AuthResult()
                {
                    Errors =
                    [
                        "Invalid Payload"
                    ],
                    Result = false

                });
            }
            var result = await _signInManager.CheckPasswordSignInAsync(existing, login.Password, false);
            if (!result.Succeeded)
                return BadRequest(new AuthResult()
                {
                    Errors =
                    [
                       "Invalid Credentials"
                    ],
                    Result = false

                });


            return Ok(_jwtProvider.GenerateTokenAsync(existing));
        }

        [HttpPost("api/refresh-token")]
        public async Task<IActionResult> RefreshToken(JwtRequest request)
        {
            var result = await VerifyAndGenerateTokenRefresh(request);
            if (result is null)
                return BadRequest(new AuthResult()
                {
                    Errors =
                    [
                        "Invalid parameters or token"
                    ],
                    Result = false

                });

            return Ok(result);
        }

        private async Task<AuthResult> VerifyAndGenerateTokenRefresh(JwtRequest request)
        {
            var handler = new JsonWebTokenHandler();
            try
            {

                var utcExpires = DateTime.UtcNow.Add(_jwtOptions.AccessTokenExpires);
                var storedToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(n => n.Token.Equals(request.RefreshJwt));

                if (storedToken is null)
                    return new AuthResult()
                    {
                        Result = false,
                        Errors =
                        [
                            "Invalid Token"
                        ]
                    };

                if (storedToken.IsUsed)
                    return new AuthResult()
                    {
                        Result = false,
                        Errors =
                        [
                            "Invalid Token"
                        ]
                    };

                if (storedToken.IsRevoked)
                    return new AuthResult()
                    {
                        Result = false,
                        Errors =
                        [
                            "Invalid Token"
                        ]
                    };

                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await _jwtProvider.GenerateTokenAsync(dbUser);



            }
            catch (Exception)
            {

                throw;
            }


        }

        [Authorize]
        [HttpGet("api/hit-me")]
        public string Get() => "You hit me!";
    }
}
