namespace TokenRefresh.Api.Models
{
    public class Team
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? TeamPrincipal { get; set; }
    }
}
