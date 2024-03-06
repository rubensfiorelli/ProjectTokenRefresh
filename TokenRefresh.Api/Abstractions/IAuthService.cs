using TokenRefresh.Api.InputModels;

namespace TokenRefresh.Api.Abstractions
{
    public interface IAuthService
    {
        Task<dynamic> RegisterUser(NewUser newUser);
        Task<bool> Login(UserLogin login);
    }
}
