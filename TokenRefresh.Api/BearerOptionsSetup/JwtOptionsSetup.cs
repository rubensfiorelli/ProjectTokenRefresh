using Microsoft.Extensions.Options;
using TokenRefresh.Api.Authentication;

namespace TokenRefresh.Api.BearerOptionsSetup
{
    public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
    {
        private const string _sectionName = "JwtOptions";
        private readonly IConfiguration _configuration;

        public JwtOptionsSetup(IConfiguration configuration) => _configuration = configuration;

        public void Configure(JwtOptions options)
        {
            _configuration.GetSection(_sectionName).Bind(options);
        }
    }
}
