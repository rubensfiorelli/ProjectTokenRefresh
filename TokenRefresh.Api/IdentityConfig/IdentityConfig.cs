using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TokenRefresh.Api.Abstractions;
using TokenRefresh.Api.Authentication;
using TokenRefresh.Api.BearerOptionsSetup;
using TokenRefresh.Api.Data;

#nullable disable
namespace TokenRefresh.Api.IdentityConfig
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache()
                    .AddDataProtection();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var key = Encoding.UTF8.GetBytes(configuration.GetSection("JwtOptions")
                .GetValue<string>("SecretKey"));

            var tokenValidationParameter = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false

            };
            services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(n =>
            {
                n.RequireHttpsMetadata = false;
                n.SaveToken = true;
                n.TokenValidationParameters = tokenValidationParameter;
            });

            services.ConfigureOptions<JwtOptionsSetup>();

            services.TryAddScoped(typeof(IJwtProvider), typeof(JwtProvider));


            return services;
        }
    }
}
