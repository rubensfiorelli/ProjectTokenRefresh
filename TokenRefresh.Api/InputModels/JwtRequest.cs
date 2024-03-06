namespace TokenRefresh.Api.InputModels
{
    public readonly record struct JwtRequest
    {
        public string Jwt { get; init; }
        public string RefreshJwt { get; init; }
    }
}
