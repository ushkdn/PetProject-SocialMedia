using Microsoft.AspNetCore.Mvc;

namespace SocialNetwork.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<GetUserDto>> Register(RegisterUserDto newUser);
        Task<ServiceResponse<string>> LoginIn(LoginInUserDto user);
    }
}

