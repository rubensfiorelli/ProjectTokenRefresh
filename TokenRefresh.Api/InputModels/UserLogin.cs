namespace TokenRefresh.Api.InputModels
{
    public readonly record struct UserLogin
    {
        public string? Email { get; init; }
        public string? Password { get; init; }
    }
}
