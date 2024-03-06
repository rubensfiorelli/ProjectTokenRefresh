namespace TokenRefresh.Api.InputModels
{
    public readonly record struct NewUser
    {
        public string Name { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
    }
}
