namespace TokenRefresh.Api.Authentication
{
    public class JwtOptions
    {
        public string? Issuer { get; init; }
        public string? Audience { get; init; }
        public string? SecretKey { get; init; }
        public TimeSpan AccessTokenExpires { get; set; }
        public TimeSpan RefreshTokenexpires { get; set; }
    }
}
