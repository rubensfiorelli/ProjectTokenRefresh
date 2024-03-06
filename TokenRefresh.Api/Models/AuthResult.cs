namespace TokenRefresh.Api.Models
{
    public class AuthResult
    {
        public string? NormalizedUsername { get; init; }
        public string? RefreshToken { get; init; }
        public string? Jwtoken { get; init; }
        public bool Result { get; init; }
        public List<string>? Errors { get; init; }
       
    }
}
